
struct A { int x; };

double Test(void)
{
    struct A; // This vacuous variable declaration will get special treatment:
              // it will declare a *new* 'struct A' in this scope, for usage in
              // 'struct B'.
    struct B { struct A *y; };
    struct A { double x; struct B *y; };
    struct B z;
    return z.y->x;
}
