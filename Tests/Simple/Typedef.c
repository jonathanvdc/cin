typedef int int32;
typedef struct { int32 x; } A;

A Test(void)
{
    A result;
    result.x = 3;
    return result;
}
