#include <stdint.h>
#include <stddef.h>

#include <stdio.h>

extern "C" int poly1305_auth(uint8_t out[16], const uint8_t *msg,
                             size_t msg_len, const uint8_t key[32]);

int main() {
  const uint8_t key[32] = {
      0x85, 0xd6, 0xbe, 0x78, 0x57, 0x55, 0x6d, 0x33,
      0x7f, 0x44, 0x52, 0xfe, 0x42, 0xd5, 0x06, 0xa8,
      0x01, 0x03, 0x80, 0x8a, 0xfb, 0x0d, 0xb2, 0xfd,
      0x4a, 0xbf, 0xf6, 0xaf, 0x41, 0x49, 0xf5, 0x1b};
  const uint8_t message[] = "Cryptographic Forum Research Group";
  const uint8_t expected[16] = {
      0xa8, 0x06, 0x1d, 0xc1, 0x30, 0x51, 0x36, 0xc6,
      0xc2, 0x2b, 0x8b, 0xaf, 0x0c, 0x01, 0x27, 0xa9};
  uint8_t tag[16];

  if (poly1305_auth(tag, message, sizeof(message) - 1, key) != 0) {
    fputs("poly1305_auth returned an error\n", stderr);
    return 1;
  }

  for (size_t i = 0; i < sizeof(expected); i++) {
    if (tag[i] != expected[i]) {
      fputs("Poly1305 RFC test vector mismatch\n", stderr);
      return 1;
    }
  }

  return 0;
}
