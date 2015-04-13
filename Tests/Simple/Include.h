#pragma once

#define pointer(T) typeof(T*)

typedef struct
{
    int x;
} A;

#if BUFWIDTH >= 1024
#else

int Dereference(pointer(A) Value)
{
    return Value->x;
}

#endif
