﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Scm.Rx.Trace;
using Scm.Sys;

namespace Scm.Rx
{
    public static class TraceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IObservable<T> Trace<T>(
            this IObservable<T> source,
            TextWriter writer,
            ICallerInfo callerInfo,
            bool? enabled = null)
            => source.Wrap(new TextWriterHereTracer(writer, callerInfo, enabled).Trace);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Pass callerinfo to target")]
        public static IObservable<T> TraceHere<T>(
            this IObservable<T> source,
            TextWriter writer = null,
            bool? enabled = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => source.Trace(writer ?? Console.Error,
                CallerInfo.Here(callerMemberName, callerFilePath, callerLineNumber), enabled);

        public static IObservable<T> Wrap<T>(
            this IObservable<T> source,
            Func<IObservable<T>, IObservable<T>> wrapper)
        {
            return wrapper == null ? source : wrapper(source);
        }
    }
}