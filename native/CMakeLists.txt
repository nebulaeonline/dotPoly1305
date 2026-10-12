cmake_minimum_required(VERSION 3.10)

project(poly1305 LANGUAGES CXX)

option(POLY1305_BUILD_TESTS "Build the native Poly1305 smoke test" OFF)

set(POLY1305_RID "" CACHE STRING "Target .NET RID")
set(BORINGSSL_SOURCE_DIR
    "${CMAKE_CURRENT_LIST_DIR}/../../boringssl-rob"
    CACHE PATH "Path to the BoringSSL source checkout")
set(POLY1305_EXPORT_SOURCE
    "${CMAKE_CURRENT_LIST_DIR}/poly1305_export.cc"
    CACHE FILEPATH "Path to the Poly1305 C ABI export source")
set(POLY1305_OUTPUT_DIRECTORY
    "${CMAKE_CURRENT_LIST_DIR}/../dotPoly1305/runtimes/${POLY1305_RID}/native"
    CACHE PATH "Directory for the native library output")

if(NOT POLY1305_RID MATCHES "^(win-x64|win-arm64|linux-x64|linux-arm64|osx-x64|osx-arm64)$")
  message(FATAL_ERROR "Set POLY1305_RID to one of the six supported RIDs.")
endif()

foreach(required_file
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305/poly1305.cc"
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305/poly1305_vec.cc"
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305/internal.h"
    "${BORINGSSL_SOURCE_DIR}/crypto/internal.h"
    "${BORINGSSL_SOURCE_DIR}/include/openssl/poly1305.h"
    "${POLY1305_EXPORT_SOURCE}")
  if(NOT EXISTS "${required_file}")
    message(FATAL_ERROR "Required Poly1305 source file not found: ${required_file}")
  endif()
endforeach()

add_library(poly1305 SHARED
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305/poly1305.cc"
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305/poly1305_vec.cc"
    "${POLY1305_EXPORT_SOURCE}")

target_include_directories(poly1305 PRIVATE
    "${BORINGSSL_SOURCE_DIR}/include"
    "${BORINGSSL_SOURCE_DIR}/crypto"
    "${BORINGSSL_SOURCE_DIR}/crypto/poly1305")

if(MSVC)
  target_compile_options(poly1305 PRIVATE /wd4244)
endif()

set_target_properties(poly1305 PROPERTIES
    CXX_STANDARD 17
    CXX_STANDARD_REQUIRED YES
    CXX_EXTENSIONS NO
    CXX_VISIBILITY_PRESET hidden
    VISIBILITY_INLINES_HIDDEN YES
    LIBRARY_OUTPUT_DIRECTORY "${POLY1305_OUTPUT_DIRECTORY}"
    RUNTIME_OUTPUT_DIRECTORY "${POLY1305_OUTPUT_DIRECTORY}")

foreach(configuration DEBUG RELEASE RELWITHDEBINFO MINSIZEREL)
  set_target_properties(poly1305 PROPERTIES
      LIBRARY_OUTPUT_DIRECTORY_${configuration} "${POLY1305_OUTPUT_DIRECTORY}"
      RUNTIME_OUTPUT_DIRECTORY_${configuration} "${POLY1305_OUTPUT_DIRECTORY}")
endforeach()

if(POLY1305_BUILD_TESTS)
  enable_testing()
  add_executable(poly1305_smoke_test poly1305_smoke_test.cc)
  target_link_libraries(poly1305_smoke_test PRIVATE poly1305)
  add_custom_command(TARGET poly1305_smoke_test POST_BUILD
      COMMAND "${CMAKE_COMMAND}" -E copy_if_different
          "$<TARGET_FILE:poly1305>"
          "$<TARGET_FILE_DIR:poly1305_smoke_test>")
  add_test(NAME poly1305_rfc_vector COMMAND poly1305_smoke_test)
endif()
