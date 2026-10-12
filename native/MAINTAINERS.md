# Native build instructions (maintainers only)

These instructions are for maintainers. The prebuilt native libraries are
distributed in the repository and in the NuGet package; package consumers do
not need to build them.

## Source and prerequisites

The build compiles only BoringSSL's Poly1305 implementation and the
[`poly1305_export.cc`](./poly1305_export.cc) shim in this folder. It does not
build or link all of BoringSSL. `boringssl-rob` is the default source directory;
it can be overridden with `BORINGSSL_SOURCE_DIR` when building against another
BoringSSL checkout.

Use a current BoringSSL checkout. Since `boringssl-rob` may contain untracked
build outputs, check `git status` before updating and do not discard those
files. The export shim is kept in this `native` folder, separate from the
BoringSSL checkout, for inspection.

The project's `boringssl-rob` checkout can be advanced with:

```sh
git -C boringssl-rob fetch origin
git -C boringssl-rob merge --ff-only origin/main
```

The build disables MSVC warning C4244 for the BoringSSL sources, matching
BoringSSL's own CMake warning configuration.

The CMake project requires CMake 3.10 or later and a C++17 compiler. Commands
below intentionally use `cmake ..` from each build directory, rather than
`cmake -S/-B`, to work with the older CMake shipped by Ubuntu 18.04.

Build artifacts are written directly to
`dotPoly1305/runtimes/<RID>/native/` with the platform-specific name expected by
the .NET P/Invoke declaration.

## Windows x64 and ARM64

On a Windows x64 machine with Visual Studio C++ tools and CMake installed, open
a Developer PowerShell from the repository root.

Build `win-x64`:

```powershell
New-Item -ItemType Directory -Force native\build-win-x64 | Out-Null
Set-Location native\build-win-x64
cmake .. -A x64 -DPOLY1305_RID=win-x64
cmake --build . --config Release --target poly1305
Get-Item ..\..\dotPoly1305\runtimes\win-x64\native\poly1305.dll
Set-Location ..\..
```

Build `win-arm64` from the same x64 machine using the Visual Studio ARM64
compiler toolchain:

```powershell
New-Item -ItemType Directory -Force native\build-win-arm64 | Out-Null
Set-Location native\build-win-arm64
cmake .. -A ARM64 -DPOLY1305_RID=win-arm64
cmake --build . --config Release --target poly1305
Get-Item ..\..\dotPoly1305\runtimes\win-arm64\native\poly1305.dll
Set-Location ..\..
```

Both builds produce `poly1305.dll` directly in the corresponding
`dotPoly1305/runtimes/<RID>/native/` directory. Visual Studio also creates
`poly1305.lib` in the build's `Release` directory; this is the DLL's import
library, not a static library. Check for the `.dll` at the path printed by
`Get-Item`, not beside the `.lib`.

## Linux x64 and ARM64

Build natively on an Ubuntu 18.04 VM of the matching architecture. Install
`build-essential` and CMake 3.10 or later. For example, Ubuntu 18.04's CMake
3.10.2 is sufficient; no newer CMake command-line options are required.

Build `linux-x64` on an x64 VM:

```sh
mkdir -p native/build-linux-x64
cd native/build-linux-x64
cmake .. -DPOLY1305_RID=linux-x64 -DCMAKE_BUILD_TYPE=Release
cmake --build . --config Release
cd ../..
```

Build `linux-arm64` on an ARM64 VM:

```sh
mkdir -p native/build-linux-arm64
cd native/build-linux-arm64
cmake .. -DPOLY1305_RID=linux-arm64 -DCMAKE_BUILD_TYPE=Release
cmake --build . --config Release
cd ../..
```

Both builds produce `libpoly1305.so`.

## macOS x64 and ARM64

Build on an Apple Silicon Mac with Xcode command-line tools and CMake installed.
Set `CMAKE_OSX_DEPLOYMENT_TARGET` on the CMake command line to ensure the
resulting binaries require no macOS version newer than 11.0.

Build `osx-arm64`:

```sh
mkdir -p native/build-osx-arm64
cd native/build-osx-arm64
cmake .. -DPOLY1305_RID=osx-arm64 \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_OSX_ARCHITECTURES=arm64 \
  -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0
cmake --build . --config Release
cd ../..
```

Build `osx-x64`:

```sh
mkdir -p native/build-osx-x64
cd native/build-osx-x64
cmake .. -DPOLY1305_RID=osx-x64 \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_OSX_ARCHITECTURES=x86_64 \
  -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0
cmake --build . --config Release
cd ../..
```

Both builds produce `libpoly1305.dylib`.

## Verify a build

Each RID must be built on a matching native host/target toolchain as described
above. The Apple Silicon VM can build both macOS architectures; Windows x64 can
cross-build Windows ARM64. Do not copy binaries across RIDs.

To build and run the RFC 7539 Poly1305 test vector on a host architecture,
configure with `-DPOLY1305_BUILD_TESTS=ON`, build, then run
`ctest -C Release --output-on-failure`.
