using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ORM
{
    /// <summary>
    /// Used to dissect LINQ expressions
    /// </summary>
    public abstract class ExpressionTreeVisitor
    {
        /// <summary>
        /// Generic expression visiter method
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual Expression Visit(Expression e)
        {
            if (e == null) return null;

            switch (e.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.OnesComplement:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Unbox:
                    {
                        return VisitUnary((UnaryExpression)e);
                    }
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    {
                        return VisitBinary((BinaryExpression)e);
                    }
                case ExpressionType.TypeIs:
                    {
                        return VisitTypeIs((TypeBinaryExpression)e);
                    }
                case ExpressionType.Conditional:
                    {
                        return VisitConditional((ConditionalExpression)e);
                    }
                case ExpressionType.Constant:
                    {
                        return VisitConstant((ConstantExpression)e);
                    }
                case ExpressionType.Parameter:
                    {
                        return VisitParameter((ParameterExpression)e);
                    }
                case ExpressionType.MemberAccess:
                    {
                        return VisitMemberAccess((MemberExpression)e);
                    }
                case ExpressionType.Call:
                    {
                        return VisitMethodCall((MethodCallExpression)e);
                    }
                case ExpressionType.Lambda:
                    {
                        return VisitLambda((LambdaExpression)e);
                    }
                case ExpressionType.New:
                    {
                        return VisitNew((NewExpression)e);
                    }
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    {
                        return VisitNewArray((NewArrayExpression)e);
                    }
                case ExpressionType.Invoke:
                    {
                        return VisitInvocation((InvocationExpression)e);
                    }
                case ExpressionType.MemberInit:
                    {
                        return VisitMemberInit((MemberInitExpression)e);
                    }
                case ExpressionType.ListInit:
                    {
                        return VisitListInit((ListInitExpression)e);
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format("Unknown expression type: '{0}'", e.NodeType));
                    }
            }
        }

        /// <summary>
        /// Used to visit a Binding
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new NotSupportedException(string.Format("Unknown binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// Used to visit an element initializer
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            return Expression.ElementInit(initializer.AddMethod,
                this.VisitExpressionList(initializer.Arguments));
        }

        /// <summary>
        /// used to visit a unary expression
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            return Expression.MakeUnary(u.NodeType, Visit(u.Operand), u.Type, u.Method);
        }

        /// <summary>
        /// used to visit a binary expression
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = Visit(b.Left);
            Expression right = Visit(b.Right);
            Expression conv = Visit(b.Conversion);

            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                return Expression.Coalesce(left, right, conv as LambdaExpression);
            else
                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }
        
        /// <summary>
        /// Used to visit a type binary expression
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            return Expression.TypeIs(Visit(b.Expression), b.TypeOperand);
        }

        /// <summary>
        /// Used to visit a constant expression
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// Used to visit a conditional expression
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            return Expression.Condition(Visit(c.Test), Visit(c.IfTrue), Visit(c.IfFalse));
        }

        /// <summary>
        /// Used to visit a parameter expression
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual ParameterExpression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// Used to visit a member access expression
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            return Expression.MakeMemberAccess(Visit(m.Expression), m.Member);
        }

        /// <summary>
        /// Used to visit a method call expression
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            return Expression.Call(Visit(m.Object), m.Method, VisitExpressionList(m.Arguments));
        }

        /// <summary>
        /// Used to visit a list of expressions
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> list)
        {
            return list.Select(e => Visit(e)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Used to visit a member assignment
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            return Expression.Bind(assignment.Member, Visit(assignment.Expression));
        }

        /// <summary>
        /// Used to visit a member member binding
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            return Expression.MemberBind(binding.Member, VisitBindingList(binding.Bindings));
        }

        /// <summary>
        /// Used to visit a member list binding
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            return Expression.ListBind(binding.Member, VisitElementInitializerList(binding.Initializers));
        }

        /// <summary>
        /// Used to visit a binding list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> list)
        {
            return list.Select(e => VisitBinding(e)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Used to visit an element initializer list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> list)
        {
            return list.Select(e => VisitElementInitializer(e)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Used to visit a parameter list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<ParameterExpression> VisitParameterList(ReadOnlyCollection<ParameterExpression> list)
        {
            return list.Select(e => VisitParameter(e)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Used to visit a lambda expression
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            return Expression.Lambda(lambda.Type, Visit(lambda.Body), VisitParameterList(lambda.Parameters));
        }

        /// <summary>
        /// Used to visit a new expression
        /// </summary>
        /// <param name="newExpression"></param>
        /// <returns></returns>
        protected virtual NewExpression VisitNew(NewExpression newExpression)
        {
            var args = VisitExpressionList(newExpression.Arguments);
            if (newExpression.Members != null)
                return Expression.New(newExpression.Constructor, args, newExpression.Members);
            else
                return Expression.New(newExpression.Constructor, args);
        }

        /// <summary>
        /// Used to visit a member init expression
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected virtual MemberInitExpression VisitMemberInit(MemberInitExpression init)
        {
            return Expression.MemberInit(VisitNew(init.NewExpression), VisitBindingList(init.Bindings));
        }

        /// <summary>
        /// Used to visit a list init expression
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected virtual ListInitExpression VisitListInit(ListInitExpression init)
        {
            return Expression.ListInit(VisitNew(init.NewExpression), VisitElementInitializerList(init.Initializers));
        }

        /// <summary>
        /// Used to visit a new array expression
        /// </summary>
        /// <param name="na"></param>
        /// <returns></returns>
        protected virtual NewArrayExpression VisitNewArray(NewArrayExpression na)
        {
            var e = VisitExpressionList(na.Expressions);
            Type type = na.Type.GetGenericArguments().Single(t => t != typeof(object));
            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(type, e);
            }
            else
            {
                return Expression.NewArrayBounds(type, e);
            }
        }

        /// <summary>
        /// Use to visit an invocation expression
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            return Expression.Invoke(Visit(iv.Expression), VisitExpressionList(iv.Arguments));
        }
    }
}
