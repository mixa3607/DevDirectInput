import struct
import time
import sys
import signal
import os
import logging
import json
import math
import datetime
import shutil
import argparse
from typing import List, Tuple, Sequence, Dict
from urllib.request import urlopen


class ProgressBar(object):
    console_length: int = 80
    """
    Create terminal progress bar
    @params:
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
        zfill       - Optional  : bar zero fill character (Str)
        file        - Optional  : output file (Stream)
    """

    def __init__(self, total, prefix='', suffix='', decimals=1, fill='█', zfill='-', file=sys.stdout):
        self.__prefix = prefix
        self.__suffix = suffix
        self.__decimals = decimals
        if shutil.get_terminal_size().columns != 0:
            self.__total_lenght = shutil.get_terminal_size().columns
        else:
            self.__total_lenght = ProgressBar.console_length
        self.__length = self.__total_lenght -(len(self.__prefix)+1) -1 -1 -(2+1+self.__decimals+1) -1 -len(self.__suffix) -2
        self.__fill = fill
        self.__zfill = zfill
        self.__total = total
        self.__iteration = 0
        self.__file = file

    def generate_pbar(self, iteration):
        """
        Create and return the progress bar string
        @params:
            iteration   - Required  : current iteration (Int)
        """
        self.__iteration = iteration
        percent = ('{0:.' + str(self.__decimals) + 'f}').format(100 * (iteration / float(self.__total)))
        filled_length = int(self.__length * iteration // self.__total)
        pbar = self.__fill * filled_length + self.__zfill * (self.__length - filled_length)
        return f'{self.__prefix} |{pbar}| {percent}% {self.__suffix}'

    def print_progress_bar(self, iteration):
        """
        Prints the progress bar
        @params:
        iteration   - Required  : current iteration (Int)
        """
        print(f'\r{self.generate_pbar(iteration)}', end='', file=self.__file)
        self.__file.flush()
        # Print New Line on Complete
        if iteration == self.__total:
            print(file=self.__file)

    def next(self):
        """Print next interation progress bar
        """
        self.__iteration += 1
        self.print_progress_bar(self.__iteration)


class EventStruct:
    input_device_format = 'llHHI'
    input_struct_size = 0

    @staticmethod
    def get_struct_size() -> int:
        if EventStruct.input_struct_size == 0:
            EventStruct.input_struct_size = struct.calcsize(
                EventStruct.input_device_format)
        return EventStruct.input_struct_size

    def __init__(self, ev_type: int, ev_code: int, ev_value: int) -> None:
        self.ev_type = ev_type
        self.ev_code = ev_code
        self.ev_value = ev_value

    def to_bytes(self) -> bytearray:
        return struct.pack(self.input_device_format, 0, 0, self.ev_type, self.ev_code, self.ev_value)


class EventReplay:
    version: int
    name: str
    author: str
    about: str
    length_secs: int
    stop_on_trigger: bool
    start_on_trigger: bool
    device_paths: List[str]
    input_device_ids: List[int]
    trigger_device_ids: List[int]
    tick_ns: float
    tick_rate: float
    updates: List[List[int]]
    post_abort_updates: List[List[int]]
    devices_fds: Dict[int, int]

    def __init__(self):
        self.devices_fds = {}

    def load_from_file(self, json_path: str) -> None:
        print('Loading replay from file... ', end='')
        with open(json_path, 'r') as fd:
            json_str = fd.read()
            self.load_from_json(json_str)
        print('OK')

    def load_from_url(self, json_url: str) -> None:
        print('Loading replay from internet... ', end='')
        data = urlopen(json_url).read()
        json = data.decode("utf-8")
        print('OK')
        self.load_from_json(json)

    def load_from_json(self, json_str: str) -> None:
        print('Parsing replay... ', end='')
        input_script = json.loads(json_str)
        self.version = input_script['version']

        self.name = input_script['name']
        self.author = input_script['author']
        self.about = input_script['about']

        self.device_paths = input_script['devicePaths']
        self.input_device_ids = input_script['inputDeviceIds']
        self.trigger_device_ids = input_script['triggerDeviceIds']
        
        self.updates = input_script['updates']
        self.post_abort_updates = input_script['postAbortUpdates']
        self.set_tick_rate(input_script['tickRate'])
        print('OK')
    
    def set_tick_rate(self, tick_rate: float) -> None:
        self.tick_rate = tick_rate
        self.tick_ns = 1 / self.tick_rate * 1000 * 1000 * 1000
        self.length_secs = int(len(self.updates) / self.tick_rate)

    def print(self, verbose: int = False) -> None:
        info_dict: Dict = {}
        if verbose >= 1:
            info_dict['Replay ver'] = self.version
        info_dict['Name'] = self.name
        info_dict['Author'] = self.author
        info_dict['About'] = self.about
        if verbose >= 1:
            info_dict['Tick rate'] = self.tick_rate
            info_dict['Updates'] = len(self.updates)
            info_dict['Post abort upds'] = len(self.post_abort_updates)
            info_dict['Start by trigger'] = self.start_on_trigger
            info_dict['Stop by trigger'] = self.stop_on_trigger
        if verbose >= 2:
            info_dict['Input devices'] = {x: self.device_paths[x] for x in self.input_device_ids}
            info_dict['Trigger devices'] = {x: self.device_paths[x] for x in self.trigger_device_ids}

        max_key_len = len(max(info_dict.keys(), key=len)) +2
        info = ''
        for key,val in info_dict.items():
            info += f'{key}:{" "*(max_key_len - len(key))}{val}\n'
        print(info)


class EventReplayPlayer:
    options: EventReplay
    raw_updates: List[List[Tuple[int, bytearray]]]
    raw_abort_updates: List[List[Tuple[int, bytearray]]]

    def __init__(self, options: EventReplay):
        self.options = options

    def open_event_fds(self) -> None:
        for trigger_device_id in self.options.trigger_device_ids:
            self.options.devices_fds[trigger_device_id] = \
                os.open(self.options.device_paths[trigger_device_id], os.O_RDONLY | os.O_NONBLOCK)

        for input_device_id in self.options.input_device_ids:
            self.options.devices_fds[input_device_id] = os.open(
                self.options.device_paths[input_device_id], os.O_WRONLY)
        logging.info('All event files opened')

    def close_event_fds(self) -> None:
        for fd in self.options.devices_fds:
            os.close(fd)

    def prepare_main_events(self) -> None:
        print('Prepearing main events... ', end='')
        self.raw_updates = self.prepare_events(self.options.updates)
        print('OK')

    def prepare_abort_events(self) -> None:
        print('Prepearing post abort events... ', end='')
        self.raw_abort_updates = \
            self.prepare_events(self.options.post_abort_updates)
        print('OK')

    @staticmethod
    def prepare_events(updates: List[List[int]]) -> List[List[Tuple[int, bytearray]]]:
        raw_updates: List[List[Tuple[int, bytearray]]] = []
        for update in updates:
            update_events: List = None
            if update != None and len(update) > 0:
                update_events = []
                for event in update:
                    update_events.append((event[0], EventStruct(
                        event[1], event[2], event[3]).to_bytes()))
            raw_updates.append(update_events)
        return raw_updates

    def check_new_events(self) -> bool:
        for device_id in self.options.trigger_device_ids:
            if self.check_fd_new_events(self.options.devices_fds[device_id], EventStruct.get_struct_size()*100):
                return True
        return False

    @staticmethod
    def check_fd_new_events(fd: int, batch_len: int) -> bool:
        hasReaded = False
        try:
            while len(os.read(fd, batch_len)) > 0:
                hasReaded = True
        except BlockingIOError:
            pass
        return hasReaded

    def execute_replay(self) -> bool: # true if need post abort events
        if self.options.start_on_trigger:
            print('\rWait trigger events... ', end='')
            while not self.check_new_events():
                time.sleep(0.2)
            else:
                time.sleep(0.7)
                self.check_new_events()
                print('OK')

        suffix = f'of {datetime.timedelta(seconds=self.options.length_secs)}'
        pb = ProgressBar(total=len(self.raw_updates), prefix='Main', suffix=suffix, decimals=2)

        for raw_update in self.raw_updates:
            start_ns = time.time_ns()
            if self.options.stop_on_trigger and self.check_new_events():
                print('\nStop replaying by trigger')
                return True
            if raw_update != None:
                for fd_id, event_bytes in raw_update:
                    os.write(self.options.devices_fds[fd_id], event_bytes)

            pb.next()            
            cost_ns = time.time_ns() - start_ns
            if cost_ns >= self.options.tick_ns:
                print(f'\nWARN: Cost time to one step more then tick: {cost_ns / 1000 / 1000 / 1000}ms')
            else:
                delay_s = (self.options.tick_ns - cost_ns) / 1000 / 1000 / 1000
                time.sleep(delay_s)
        return False
    
    def execute_post_abort_replay(self) -> None:
        suffix = f'of {len(self.raw_abort_updates)}'
        pb = ProgressBar(total=len(self.raw_abort_updates), prefix='Abort', suffix=suffix, decimals=2)

        for raw_update in self.raw_abort_updates:
            if raw_update != None:
                for fd_id, event_bytes in raw_update:
                    os.write(self.options.devices_fds[fd_id], event_bytes)
            pb.next()

    def execute(self) -> None:
        self.open_event_fds()
        self.prepare_main_events()
        self.prepare_abort_events()
        self.execute_replay()
        self.close_event_fds()


def get_args() -> any:
    parser = argparse.ArgumentParser(description='Program for playing replays generated by DDI.\nhttps://github.com/mixa3607/DevDirectInput')
    parser.add_argument('-r', '--replay', action='store',
                        dest='replay', required=True, help='Path to replay')
    parser.add_argument('-t', '--tickrate', action='store',
                        dest='tick_rate', required=False, type=float, help='Override tickrate')
    parser.add_argument('-b', '--begin', action='store_true',
                        dest='begin', required=False, help='Start by trigger')
    parser.add_argument('-a', '--abort', action='store_true',
                        dest='abort', required=False, help='Stop by trigger')
    parser.add_argument('-f', '--flush', action='store_true',
                        dest='flush', required=False, help='Execute post abort events and exit')
    parser.add_argument('-i', '--info', action='store_true',
                        dest='info_only', required=False, help='Only print info')
    parser.add_argument('-v', '--verbose', action='count',
                        dest='verbose', required=False, default=0, help='Verbose (v|vv)')
    parser.add_argument('--ccol', action='store', type=int,
                        dest='console_columns', help='Use if have problem with progress bars', default=80)
    return parser.parse_args()

if __name__ == "__main__":
    args = get_args()
    event_replay = EventReplay()
    event_replay.start_on_trigger = args.begin
    event_replay.stop_on_trigger = args.abort
    ProgressBar.console_length = args.console_columns
    replay_uri: str = args.replay
    if replay_uri.startswith(('https://', 'http://')):
        event_replay.load_from_url(replay_uri)
    else:
        event_replay.load_from_file(replay_uri)

    if args.tick_rate:
        event_replay.set_tick_rate(args.tick_rate)
        print(f'Warn: Tickrate was overrided to {event_replay.tick_rate}')
    event_replay.print(args.verbose)
    if args.info_only:
        exit()
    
    event_replay_player = EventReplayPlayer(event_replay)
    event_replay_player.open_event_fds()
    event_replay_player.prepare_main_events()
    event_replay_player.prepare_abort_events()
    if args.flush:
        event_replay_player.execute_post_abort_replay()
        exit()
    if event_replay_player.execute_replay():
        event_replay_player.execute_post_abort_replay()
    event_replay_player.close_event_fds()
