import yaml
import dir_helper
import os

NEW_PATH = "../../Pokemon Unity/Assets/Scripts/"
OLD_PATH = "./old/"
DONE_PATH = "./done/"

YAML_HEADER = "%YAML 1.1\n\
%TAG !u! tag:unity3d.com,2011:\n\
--- !u!114 &11400000\n"

WRAPPER_KEY = "MonoBehaviour"

SKIP_KEYS = [
    "m_ObjectHideFlags",
    "m_CorrespondingSourceObject",
    "m_PrefabInstance",
    "m_PrefabAsset",
    "m_GameObject",
    "m_Enabled",
    "m_EditorHideFlags",
    "m_Script",
    "m_Name",
    "m_EditorClassIdentifier",
    "Id",
]

def load_yaml(path):
    with open(path, 'r') as file:
        file.readline()
        file.readline()
        file.readline()
        return yaml.safe_load(file)
    
def save_yaml(path, data):
    dir_helper.create_dir(os.path.dirname(path))
    with open(path, 'w') as file:
        file.write(YAML_HEADER)
        yaml.dump(data, file)

def old_key_to_new(key):
    if key == "fullName":
        key = "name"

    return f"<{key[0].upper()}{key[1:]}>k__BackingField"

paths = []

for path, subdirs, files in os.walk(OLD_PATH):
    for name in files:
        if name.split(".")[-1] != "asset":
            continue
        old_path = os.path.join(path, name).replace(OLD_PATH, "")
        print(old_path)
        paths.append(old_path)

for path in paths:
    new_path = os.path.join(NEW_PATH, path)
    old_path = os.path.join(OLD_PATH, path)
    done_path = os.path.join(DONE_PATH, path)

    new_data = load_yaml(new_path)
    old_data = load_yaml(old_path)

    # print(new_data)
    # print("")
    # print(old_data)


    for key in old_data[WRAPPER_KEY]:
        if key in SKIP_KEYS:
            continue

        key_new = old_key_to_new(key)

        if key_new not in new_data[WRAPPER_KEY]:
            if key in new_data[WRAPPER_KEY]:
                key_new = key
            else:
                print(path)
                print("\tNot found:", key_new, key)
                continue

        new_data[WRAPPER_KEY][key_new] = old_data[WRAPPER_KEY][key]
        

    save_yaml(done_path, new_data)
    save_yaml(old_path, new_data)


