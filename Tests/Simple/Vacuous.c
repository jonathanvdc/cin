
struct A { int x; };

double Test(void)
{
    struct A;
    struct B { struct A y; };
    struct A { double x; };
    struct B z;
    return z.y.x;
}
