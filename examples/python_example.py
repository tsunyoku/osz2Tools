from ctypes import CDLL, c_char_p

if sys.platform == "win32":
    dll_path = "./osz2Tools"
else:
    dll_path = "./osz2Tools.so"

tools_dll = CDLL(dll_path)
decrypt_osz2_func = tools_dll.decrypt_osz2
decrypt_patch_func = tools_dll.decrypt_patch

def decrypt_osz2(osz2_path: str, target_path: str) -> None:
    osz2_ptr = c_char_p(osz2_path.encode())
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_osz2_func(osz2_ptr, target_ptr)
    
def decrypt_patch(patch_path: str, old_osz2_path: str, target_path: str) -> None:
    patch_ptr = c_char_p(patch_path.encode())
    old_osz2_ptr = c_char_p(old_osz2_path.encode())
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_patch_func(patch_ptr, old_osz2_ptr, target_ptr)

if __name__ == "__main__":
    decrypt_osz2("test.osz2", "output")
    decrypt_patch("test.patch", "test.osz2", "output")