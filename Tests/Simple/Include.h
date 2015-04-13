#pragma once

#define pointer(T) typeof(T*)

typedef struct
{
    int x;
} A;

#if !defined(pointer) || BUFWIDTH >= 1024 // This is a comment!
#else

int Dereference(pointer(A) Value)
{
    return Value->x;
}

#endif
