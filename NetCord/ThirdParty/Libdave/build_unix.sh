#!/bin/sh

BASEDIR=$(dirname "$0")

CMakeArgs="-DCMAKE_CXX_COMPILER_LAUNCHER=ccache" dotnet msbuild -restore -tl:off -tlp:v=n -v:n $BASEDIR/Libdave.RID/Libdave.RID.csproj
