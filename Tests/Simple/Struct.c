
struct A b;

struct A
{
    int x, y;
    char *z;
};

int Test(void)
{
    b.x = 5;
    b.y = 6;
    char local = 5;
    b.z = (uint8_t*)&local;
    return b.x;
}
