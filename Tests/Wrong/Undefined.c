
struct A; // Declare, but don't define.

int Test(void)
{
    struct A b;
    return b.x; // There is no way this can be resolved.
}
