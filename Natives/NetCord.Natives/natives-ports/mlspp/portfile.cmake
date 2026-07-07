vcpkg_from_github(
    OUT_SOURCE_PATH SOURCE_PATH
    REPO cisco/mlspp
    REF "${VERSION}"
    SHA512 5d37631e2c47daae1133ef074e60cc09ca2d395f9e11c416f829060e374051cf219d2d7fe98dae49d1d045292e07d6a09f4814a5f16e6cc05e67e7cd96f146c4
)

if(VCPKG_TARGET_IS_OSX AND EXISTS "/usr/local/include/openssl/")
    set(VCPKG_INCLUDE_OVERRIDE "-DCMAKE_CXX_FLAGS=-I${CURRENT_INSTALLED_DIR}/include")
endif()

set(VCPKG_LIBRARY_LINKAGE static)

vcpkg_cmake_get_vars(cmake_vars_file)
include("${cmake_vars_file}")

set(EXTRA_OPTIONS -DDISABLE_GREASE=ON -DTESTING=OFF -DBUILD_TESTING=OFF -DMLS_CXX_NAMESPACE="mlspp")
if(VCPKG_HOST_IS_LINUX)
    if(VCPKG_DETECTED_CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
        # Safe for GCC: Apply both flags safely as a single quoted string
        list(APPEND EXTRA_OPTIONS "-DCMAKE_CXX_FLAGS=-Wno-error=maybe-uninitialized -Wno-error=uninitialized")
    else()
        # Safe for Clang: Only apply the base uninitialized flag
        list(APPEND EXTRA_OPTIONS "-DCMAKE_CXX_FLAGS=-Wno-error=uninitialized")
    endif()
endif()

vcpkg_cmake_configure(
    SOURCE_PATH "${SOURCE_PATH}"
    OPTIONS
        -DCMAKE_POSITION_INDEPENDENT_CODE=ON
        ${VCPKG_INCLUDE_OVERRIDE}
        ${EXTRA_OPTIONS}
    MAYBE_UNUSED_VARIABLES
        BUILD_TESTING
)

vcpkg_cmake_install()
vcpkg_copy_pdbs()

vcpkg_cmake_config_fixup(PACKAGE_NAME "MLSPP" CONFIG_PATH "share/MLSPP")

# Remove redundant debug directories to comply with vcpkg policy
file(REMOVE_RECURSE "${CURRENT_PACKAGES_DIR}/debug/include")
file(REMOVE_RECURSE "${CURRENT_PACKAGES_DIR}/debug/share")
