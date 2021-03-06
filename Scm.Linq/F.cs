﻿using System;
using System.Linq.Expressions;

namespace Scm.Linq
{
    /// <summary>
    /// Allow resolution of <code>Func&lt;...&gt;</code>/<code>Expression&lt;Func&lt;...&gt;&gt;</code>.
    /// 
    /// Useful for assignments like:
    /// var asExpr = F.Expr(x => x + 3);
    /// asExpr.Should().BeOfType&lt;Expression&lt;Func&lt;int&gt;&gt;&gt;();
    /// var asFunc = F.Func(x => x + 3);
    /// asExpr.Should().BeOfType&lt;Func&lt;int&gt;&gt;();
    /// </summary>
    public static class F
    {
        public static Expression<Func<T>> Expr<T>(this Expression<Func<T>> f) => f;
        public static Func<T> Func<T>(this Func<T> f) => f;
        public static Action Action(this Action f) => f;
        public static Expression<Func<TArg1, T>> Expr<TArg1, T>(this Expression<Func<TArg1, T>> f) => f;
        public static Func<TArg1, T> Func<TArg1, T>(this Func<TArg1, T> f) => f;

        public static Expression<Func<TArg1, TArg2, T>>
            Expr<TArg1, TArg2, T>(this Expression<Func<TArg1, TArg2, T>> f) => f;

        public static Func<TArg1, TArg2, T> Func<TArg1, TArg2, T>(this Func<TArg1, TArg2, T> f) => f;

        public static Expression<Func<TArg1, TArg2, TArg3, T>> Expr<TArg1, TArg2, TArg3, T>(
            this Expression<Func<TArg1, TArg2, TArg3, T>> f) => f;

        public static Func<TArg1, TArg2, TArg3, T> Func<TArg1, TArg2, TArg3, T>(this Func<TArg1, TArg2, TArg3, T> f) =>
            f;

        /// <summary>
        /// Creates an expression comparing two instances of <typeparamref name="T"/>
        /// </summary>
        public static Expression<Func<T, T, bool>> Eq<T>()
        {
            var t = typeof(T);
            if (!t.IsPrimitive)
                throw new NotSupportedException($"Cannot equilityc compare {t}");
            var left = Expression.Parameter(t, "left");
            var right = Expression.Parameter(t, "right");

            return Expression.Lambda<Func<T, T, bool>>(Expression.Equal(left, right), left, right);

        }

        public static Expression<Func<T, bool>> Eq<T>(T id)
        {
            var t = typeof(T);
            if (!t.IsValueType)
                throw new NotSupportedException($"Cannot equilityc compare {t}");
            var left = Expression.Constant(id, t);
            var right = Expression.Parameter(t, "right");

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), right);
        }

    }
}