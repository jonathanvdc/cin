
struct A { int* x; } b;

struct A Test(void)
{
    typeof(struct A) c;
    return c;
}

void Test2(typeof(*b.x) Value)
{
    *b.x = Value;
}
