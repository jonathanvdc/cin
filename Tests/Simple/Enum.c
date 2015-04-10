
enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
};

typedef enum
{
    Negate,
    Invert
} UnaryOperator;

enum BinaryOperator Test(void)
{
    return Add;
}

UnaryOperator Test2(void)
{
    return Negate;
}
