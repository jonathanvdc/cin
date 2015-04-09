
struct A
{
    int x;
    int y;
} a;

int GetTypeSize(void)
{
    return sizeof(struct A);
}

int GetExpressionSize(void)
{
    return sizeof a;
}

int GetArrayExpressionSize(void)
{
    struct A arr[3];
    return sizeof arr;
}

int GetArrayTypeSize(void)
{
    return sizeof(struct A[3]);
}

int GetPointerExpressionSize(void)
{
    char val = 3;
    return sizeof(&val);
}

int GetPointerTypeSize(void)
{
    return sizeof(void*);
}
