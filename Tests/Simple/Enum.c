
#define pointer(T)typeof(T*)
#define array(T, N)typeof(T[N])

enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
};

typedef enum
{
    Negate = (signed char)16,
    Invert = (short)256
} UnaryOperator;

typedef pointer(UnaryOperator) UnaryOpPtr;
#define UNARYOPPTR UnaryOpPtr

enum BinaryOperator Test(void)
{
    return Add;
}

UnaryOperator Test2(void)
{
    return Negate;
}

UNARYOPPTR Test3(void)
{
    return 0;
}
