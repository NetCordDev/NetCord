vcpkg_from_github(
    OUT_SOURCE_PATH SOURCE_PATH
    REPO discord/libdave
    REF "${VERSION}"
    SHA512 78b4e5b8ddc6397775d403465e0da770ec7905d7913546b3aec161baf4478443e554f0ae7bd012af8bfd308639be2601d46da22c02aff2b756ff91878f1fc843
    HEAD_REF main
)

vcpkg_cmake_configure(
    SOURCE_PATH "${SOURCE_PATH}/cpp"
    OPTIONS
        -DTESTING=OFF
        -DBUILD_TESTING=OFF
    MAYBE_UNUSED_VARIABLES
        BUILD_TESTING
)

vcpkg_cmake_build()

vcpkg_cmake_install()
vcpkg_copy_pdbs()

# file(REMOVE_RECURSE "${CURRENT_PACKAGES_DIR}/debug/include")

# file(INSTALL
#     "${SOURCE_PATH}/LICENSE"
#     DESTINATION "${CURRENT_PACKAGES_DIR}/share/${PORT}"
#     RENAME LICENSE.txt
# )