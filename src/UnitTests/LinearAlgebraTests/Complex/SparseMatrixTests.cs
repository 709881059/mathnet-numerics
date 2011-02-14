﻿// <copyright file="SparseMatrixTests.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// Copyright (c) 2009-2010 Math.NET
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using LinearAlgebra.Complex;
    using NUnit.Framework;

    /// <summary>
    /// Sparse matrix tests.
    /// </summary>
    public class SparseMatrixTests : MatrixTests
    {
        /// <summary>
        /// Creates a matrix for the given number of rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <returns>A matrix with the given dimensions.</returns>
        protected override Matrix CreateMatrix(int rows, int columns)
        {
            return new SparseMatrix(rows, columns);
        }

        /// <summary>
        /// Creates a matrix from a 2D array.
        /// </summary>
        /// <param name="data">The 2D array to create this matrix from.</param>
        /// <returns>A matrix with the given values.</returns>
        protected override Matrix CreateMatrix(Complex[,] data)
        {
            return new SparseMatrix(data);
        }

        /// <summary>
        /// Creates a vector of the given size.
        /// </summary>
        /// <param name="size">The size of the vector to create.
        /// </param>
        /// <returns>The new vector. </returns>
        protected override Vector CreateVector(int size)
        {
            return new SparseVector(size);
        }

        /// <summary>
        /// Creates a vector from an array.
        /// </summary>
        /// <param name="data">The array to create this vector from.</param>
        /// <returns>The new vector. </returns>
        protected override Vector CreateVector(Complex[] data)
        {
            return new SparseVector(data);
        }

        /// <summary>
        /// Can create a matrix form array.
        /// </summary>
        [Test]
        public void CanCreateMatrixFrom1DArray()
        {
            var testData = new Dictionary<string, Matrix>
                           {
                               { "Singular3x3", new SparseMatrix(3, 3, new[] { new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(2.0, 1), new Complex(2.0, 1), new Complex(2.0, 1) }) }, 
                               { "Square3x3", new SparseMatrix(3, 3, new[] { new Complex(-1.1, 1), Complex.Zero, new Complex(-4.4, 1), new Complex(-2.2, 1), new Complex(1.1, 1), new Complex(5.5, 1), new Complex(-3.3, 1), new Complex(2.2, 1), new Complex(6.6, 1) }) }, 
                               { "Square4x4", new SparseMatrix(4, 4, new[] { new Complex(-1.1, 1), Complex.Zero, new Complex(1.0, 1), new Complex(-4.4, 1), new Complex(-2.2, 1), new Complex(1.1, 1), new Complex(2.1, 1), new Complex(5.5, 1), new Complex(-3.3, 1), new Complex(2.2, 1), new Complex(6.2, 1), new Complex(6.6, 1), new Complex(-4.4, 1), new Complex(3.3, 1), new Complex(4.3, 1), new Complex(-7.7, 1) }) }, 
                               { "Tall3x2", new SparseMatrix(3, 2, new[] { new Complex(-1.1, 1), Complex.Zero, new Complex(-4.4, 1), new Complex(-2.2, 1), new Complex(1.1, 1), new Complex(5.5, 1) }) }, 
                               { "Wide2x3", new SparseMatrix(2, 3, new[] { new Complex(-1.1, 1), Complex.Zero, new Complex(-2.2, 1), new Complex(1.1, 1), new Complex(-3.3, 1), new Complex(2.2, 1) }) }
                           };

            foreach (var name in testData.Keys)
            {
                Assert.AreEqual(TestMatrices[name], testData[name]);
            }
        }

        /// <summary>
        /// Matrix from array is a copy.
        /// </summary>
        [Test]
        public void MatrixFrom1DArrayIsCopy()
        {
            // Sparse Matrix copies values from Complex[], but no remember reference. 
            var data = new[] { new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(1.0, 1), new Complex(2.0, 1), new Complex(2.0, 1), new Complex(2.0, 1) };
            var matrix = new SparseMatrix(3, 3, data);
            matrix[0, 0] = new Complex(10.0, 1);
            Assert.AreNotEqual(new Complex(10.0, 1), data[0]);
        }

        /// <summary>
        /// Matrix from two-dimensional array is a copy.
        /// </summary>
        [Test]
        public void MatrixFrom2DArrayIsCopy()
        {
            var matrix = new SparseMatrix(TestData2D["Singular3x3"]);
            matrix[0, 0] = new Complex(10.0, 1);
            Assert.AreEqual(new Complex(1.0, 1), TestData2D["Singular3x3"][0, 0]);
        }

        /// <summary>
        /// Can create a matrix from two-dimensional array.
        /// </summary>
        /// <param name="name">Matrix name.</param>
        [Test]
        public void CanCreateMatrixFrom2DArray([Values("Singular3x3", "Singular4x4", "Square3x3", "Square4x4", "Tall3x2", "Wide2x3")] string name)
        {
            var matrix = new SparseMatrix(TestData2D[name]);
            for (var i = 0; i < TestData2D[name].GetLength(0); i++)
            {
                for (var j = 0; j < TestData2D[name].GetLength(1); j++)
                {
                    Assert.AreEqual(TestData2D[name][i, j], matrix[i, j]);
                }
            }
        }

        /// <summary>
        /// Can create a matrix with uniform values.
        /// </summary>
        [Test]
        public void CanCreateMatrixWithUniformValues()
        {
            var matrix = new SparseMatrix(10, 10, new Complex(10.0, 1));
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], new Complex(10.0, 1));
                }
            }
        }

        /// <summary>
        /// Can create an identity matrix.
        /// </summary>
        [Test]
        public void CanCreateIdentity()
        {
            var matrix = SparseMatrix.Identity(5);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(i == j ? Complex.One : Complex.Zero, matrix[i, j]);
                }
            }
        }

        /// <summary>
        /// Identity with wrong order throws <c>ArgumentOutOfRangeException</c>.
        /// </summary>
        /// <param name="order">The size of the square matrix</param>
        [Test]
        public void IdentityWithWrongOrderThrowsArgumentOutOfRangeException([Values(0, -1)] int order)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SparseMatrix.Identity(order));
        }

        /// <summary>
        /// Can create a large sparse matrix
        /// </summary>
        [Test]
        public void CanCreateLargeSparseMatrix()
        {
            var matrix = new SparseMatrix(500, 1000);
            var nonzero = 0;
            var rnd = new Random();

            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    var value = rnd.Next(10) * rnd.Next(10) * rnd.Next(10) * rnd.Next(10) * rnd.Next(10);
                    if (value != 0)
                    {
                        nonzero++;
                    }

                    matrix[i, j] = value;
                }
            }

            Assert.AreEqual(matrix.NonZerosCount, nonzero);
        }
    }
}