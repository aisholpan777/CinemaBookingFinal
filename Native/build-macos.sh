#!/bin/sh
set -eu
cd "$(dirname "$0")/.."
mkdir -p native-out
clang -std=c11 -O2 -dynamiclib Native/cinemafee.c -o native-out/libcinemafee.dylib
mkdir -p bin/Debug/net8.0
cp native-out/libcinemafee.dylib bin/Debug/net8.0/
echo "Built native-out/libcinemafee.dylib"
