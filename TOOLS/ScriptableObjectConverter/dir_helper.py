import os
import datetime


def create_dir(dir):
    existing = os.path.isdir(dir)
    if not existing:
        try:
            os.makedirs(dir)
        except:
            print("Could not create", dir)


def file_exists(path):
    return os.path.isfile(path)


def dir_exists(path):
    return os.path.isdir(path)


def file_exists_deprecating(path, deprecation_time):
    if file_exists(path):
        time = datetime.datetime.utcfromtimestamp(get_mtime(path))
        if datetime.datetime.now() - time < deprecation_time:
            print("Datei noch im Cache: '{}'".format(path))
            return True
    return False


def get_mtime(path):
    return os.path.getmtime(path)
