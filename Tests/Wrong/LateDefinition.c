
// This test is technically an invalid program,
// but cin should be able to compile this.

int Test(void)
{
    struct A b; // Declare struct *and variable*.
    struct A { int x; }; // Define struct.
    b.x = 5; // Use definition.
    return b.x;
}
