using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ORM.Attributes;
using ORM.Utilities;
using SqlStatementBuilder;
using SqlStatementBuilder.Interfaces;

namespace ORM
{
    internal class SqlGeneratingExpressionTreeVisitor : ExpressionTreeVisitor
    {
        public ISqlStatementBuilder SqlStatementBuilder = new MySqlStatementBuilder();

        public override Expression Visit(Expression e)
        {
            return base.Visit(e);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            //its a variable
            if (m.Expression is ConstantExpression expression &&
                m.Member is FieldInfo info)
            {
                var container = expression.Value;
                var value = info.GetValue(container);
                SqlStatementBuilder.AddIntToCondition((int)value);
                return base.VisitMemberAccess(m);
            }

            var columnAttribute = m.Member.GetCustomAttribute<ColumnAttribute>();

            SqlStatementBuilder.AddColumnToCondition(columnAttribute != null
                ? columnAttribute.ColumnName : m.Member.Name);

            return base.VisitMemberAccess(m);
        }


        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            return base.VisitMethodCall(methodCallExpression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (!c.Value.GetType().IsPrimitive && !(c.Value is string))
            {
                var tableType = c.Value.GetType().GetGenericArguments().FirstOrDefault();

                var isVariable = c.Type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
                if (isVariable)
                {
                    return base.VisitConstant(c);
                }

                SqlStatementBuilder.TableType = c.Value.GetType();
                SqlStatementBuilder.TableName = OrmUtilities.GetTableName(tableType);
                SqlStatementBuilder.Columns = OrmUtilities.GetColumns(tableType);


            }
            else
            {
                switch (c.Value)
                {
                    case string s:
                        SqlStatementBuilder.AddStringToCondition(s);
                        break;
                    case int i:
                        SqlStatementBuilder.AddIntToCondition(i);
                        break;
                }
            }


            return base.VisitConstant(c);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            SqlStatementBuilder.StartCondition();
            Visit(b.Left);
            SqlStatementBuilder.AddBinaryOperatorToCondition(b.NodeType);
            Visit(b.Right);
            SqlStatementBuilder.EndCondition();
            return b;
        }
    }
}
