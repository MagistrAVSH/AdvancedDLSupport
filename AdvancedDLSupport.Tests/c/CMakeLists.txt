cmake_minimum_required(VERSION 3.9)
project(c)

set(CMAKE_C_STANDARD 11)
set(CMAKE_CXX_STANDARD 11)

set(CMAKE_C_COMPILER /usr/bin/clang)
set(CMAKE_CXX_COMPILER /usr/bin/clang++)

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Wall -Wno-long-long -pedantic")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wno-long-long -pedantic")

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY  ${CMAKE_BINARY_DIR}/lib)

include_directories(${CMAKE_SOURCE_DIR})

set(SHARED_HEADERS src/TestStruct.h)

add_library(BaseTests SHARED src/BaseTests.c ${SHARED_HEADERS})
add_library(DisposeTests SHARED src/DisposeTests.c ${SHARED_HEADERS})
add_library(EventTests SHARED src/EventTests.c ${SHARED_HEADERS})
add_library(FunctionTests SHARED src/FunctionTests.c ${SHARED_HEADERS})
add_library(LazyLoadingTests SHARED src/LazyLoadingTests.c ${SHARED_HEADERS})
add_library(PropertyTests SHARED src/PropertyTests.c ${SHARED_HEADERS})
add_library(RemappingTests SHARED src/RemappingTests.c ${SHARED_HEADERS})