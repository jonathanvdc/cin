
#define pointer(T) typeof(T*)

typedef struct
{
    int x;
} A;

int Dereference(pointer(A) Value)
{
    return Value->x;
}
