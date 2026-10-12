// poly1305_export.cc
#include <stdint.h>
#include <stddef.h>

#include <openssl/poly1305.h>

#if defined(_WIN32)
  #define EXPORT __declspec(dllexport)
#else
  #define EXPORT __attribute__((visibility("default")))
#endif

extern "C" {

  EXPORT int poly1305_auth(
      uint8_t out[16],
      const uint8_t* msg,
      size_t msg_len,
      const uint8_t key[32]) {
    poly1305_state state;
    CRYPTO_poly1305_init(&state, key);
    CRYPTO_poly1305_update(&state, msg, msg_len);
    CRYPTO_poly1305_finish(&state, out);
    return 0;
  }

}
