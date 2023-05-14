from ctypes import CDLL, c_char_p, c_byte
import sys

if sys.platform == "win32":
    dll_path = "./osz2Tools"
else:
    dll_path = "./osz2Tools.so"

tools_dll = CDLL(dll_path)
decrypt_osz2_func = tools_dll.decrypt_osz2
decrypt_patch_func = tools_dll.decrypt_patch
decrypt_osz2_bytes_func = tools_dll.decrypt_osz2_bytes
decrypt_patch_bytes_func = tools_dll.decrypt_patch_bytes

def decrypt_osz2(osz2_path: str, target_path: str) -> None:
    osz2_ptr = c_char_p(osz2_path.encode())
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_osz2_func(osz2_ptr, target_ptr)

def decrypt_osz2_bytes(osz2_bytes: bytearray, target_path: str) -> None:
    c_byte_array = (c_byte * len(osz2_bytes)).from_buffer(osz2_bytes)
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_osz2_bytes_func(c_byte_array, len(osz2_bytes), target_ptr)

def decrypt_patch(patch_path: str, old_osz2_path: str, target_path: str) -> None:
    patch_ptr = c_char_p(patch_path.encode())
    old_osz2_ptr = c_char_p(old_osz2_path.encode())
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_patch_func(patch_ptr, old_osz2_ptr, target_ptr)

def decrypt_patch_bytes(patch_bytes: bytearray, old_osz2_bytes: bytearray, target_path: str) -> None:
    patch_c_byte_array = (c_byte * len(patch_bytes)).from_buffer(patch_bytes)
    old_osz2_c_byte_array = (c_byte * len(old_osz2_bytes)).from_buffer(old_osz2_bytes)
    target_ptr = c_char_p(target_path.encode())
    
    decrypt_patch_bytes_func(
        patch_c_byte_array,
        len(patch_bytes),
        old_osz2_c_byte_array,
        len(old_osz2_bytes),
        target_ptr,
    )

if __name__ == "__main__":
    decrypt_osz2_bytes(bytearray(open("test.osz2", "rb").read()), "output")
    decrypt_patch_bytes(bytearray(open("test.patch", "rb").read()), bytearray(open("test.osz2", "rb").read()), "output")
