#!/bin/sh
set -eu
cd "$(dirname "$0")/.."
mkdir -p native-out
cc -std=c11 -O2 -fPIC -shared Native/cinemafee.c -o native-out/libcinemafee.so
mkdir -p bin/Debug/net8.0
cp native-out/libcinemafee.so bin/Debug/net8.0/
echo "Built native-out/libcinemafee.so"
