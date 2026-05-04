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

vcpkg_cmake_configure(
    SOURCE_PATH "${SOURCE_PATH}"
    OPTIONS
        ${VCPKG_INCLUDE_OVERRIDE}
        -DDISABLE_GREASE=ON
        -DTESTING=OFF
        -DBUILD_TESTING=OFF
        -DMLS_CXX_NAMESPACE="mlspp"
    MAYBE_UNUSED_VARIABLES
        BUILD_TESTING
)

vcpkg_cmake_install()
vcpkg_copy_pdbs()

vcpkg_cmake_config_fixup(PACKAGE_NAME "MLSPP" CONFIG_PATH "share/MLSPP")

# Remove redundant debug directories to comply with vcpkg policy
file(REMOVE_RECURSE "${CURRENT_PACKAGES_DIR}/debug/include")
file(REMOVE_RECURSE "${CURRENT_PACKAGES_DIR}/debug/share")
