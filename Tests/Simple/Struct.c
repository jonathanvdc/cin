
struct A b;

struct A
{
    int x, y;
    void *z;
};

int Test(void)
{
    b.x = 5;
    b.y = 6;
    unsigned char local = 5;
    b.z = (void*)&local;
    return b.x;
}
