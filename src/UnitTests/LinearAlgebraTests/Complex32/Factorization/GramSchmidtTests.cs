﻿// <copyright file="GramSchmidtTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex32.Factorization
{
    using System;
    using LinearAlgebra.Complex32;
    using LinearAlgebra.Complex32.Factorization;
    using LinearAlgebra.Generic.Factorization;
    using NUnit.Framework;
    using Complex32 = Numerics.Complex32;

    /// <summary>
    /// GramSchmidt factorization tests for a dense matrix.
    /// </summary>
    public class GramSchmidtTests
    {
        /// <summary>
        /// Constructor with <c>null</c> throws <c>ArgumentNullException</c>.
        /// </summary>
        [Test]
        public void ConstructorNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DenseGramSchmidt(null));
        }

        /// <summary>
        /// Constructor with wide matrix throws <c>ArgumentException</c>.
        /// </summary>
        [Test]
        public void ConstructorWideMatrixThrowsInvalidMatrixOperationException()
        {
            Assert.Throws<ArgumentException>(() => new DenseGramSchmidt(new DenseMatrix(3, 4)));
        }

        /// <summary>
        /// Can factorize identity matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanFactorizeIdentity([Values(1, 10, 100)] int order)
        {
            var matrixI = DenseMatrix.Identity(order);
            var factorGramSchmidt = matrixI.GramSchmidt();

            Assert.AreEqual(matrixI.RowCount, factorGramSchmidt.Q.RowCount);
            Assert.AreEqual(matrixI.ColumnCount, factorGramSchmidt.Q.ColumnCount);

            for (var i = 0; i < factorGramSchmidt.R.RowCount; i++)
            {
                for (var j = 0; j < factorGramSchmidt.R.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        Assert.AreEqual(Complex32.One, factorGramSchmidt.R[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(Complex32.Zero, factorGramSchmidt.R[i, j]);
                    }
                }
            }

            for (var i = 0; i < factorGramSchmidt.Q.RowCount; i++)
            {
                for (var j = 0; j < factorGramSchmidt.Q.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        Assert.AreEqual(Complex32.One, factorGramSchmidt.Q[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(Complex32.Zero, factorGramSchmidt.Q[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Identity determinant is one.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void IdentityDeterminantIsOne([Values(1, 10, 100)] int order)
        {
            var matrixI = DenseMatrix.Identity(order);
            var factorGramSchmidt = matrixI.GramSchmidt();
            Assert.AreEqual(Complex32.One, factorGramSchmidt.Determinant);
        }

        /// <summary>
        /// Can factorize a random matrix.
        /// </summary>
        /// <param name="row">Matrix row number.</param>
        /// <param name="column">Matrix column number.</param>
        [Test, Sequential]
        public void CanFactorizeRandomMatrix([Values(1, 2, 5, 10, 50, 100)] int row, [Values(1, 2, 5, 6, 48, 98)] int column)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(row, column);
            var factorGramSchmidt = matrixA.GramSchmidt();

            // Make sure the Q has the right dimensions.
            Assert.AreEqual(row, factorGramSchmidt.Q.RowCount);
            Assert.AreEqual(column, factorGramSchmidt.Q.ColumnCount);

            // Make sure the R has the right dimensions.
            Assert.AreEqual(column, factorGramSchmidt.R.RowCount);
            Assert.AreEqual(column, factorGramSchmidt.R.ColumnCount);

            // Make sure the R factor is upper triangular.
            for (var i = 0; i < factorGramSchmidt.R.RowCount; i++)
            {
                for (var j = 0; j < factorGramSchmidt.R.ColumnCount; j++)
                {
                    if (i > j)
                    {
                        Assert.AreEqual(Complex32.Zero, factorGramSchmidt.R[i, j]);
                    }
                }
            }

            // Make sure the Q*R is the original matrix.
            var matrixQfromR = factorGramSchmidt.Q * factorGramSchmidt.R;
            for (var i = 0; i < matrixQfromR.RowCount; i++)
            {
                for (var j = 0; j < matrixQfromR.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixA[i, j].Real, matrixQfromR[i, j].Real, 1e-3f);
                    Assert.AreEqual(matrixA[i, j].Imaginary, matrixQfromR[i, j].Imaginary, 1e-3f);
                }
            }

            // Make sure the Q is unitary --> (Q*)x(Q) = I
            var matrixQсtQ = factorGramSchmidt.Q.ConjugateTranspose() * factorGramSchmidt.Q;
            for (var i = 0; i < matrixQсtQ.RowCount; i++)
            {
                for (var j = 0; j < matrixQсtQ.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        Assert.AreEqual(matrixQсtQ[i, j].Real, 1.0f, 1e-3f);
                        Assert.AreEqual(matrixQсtQ[i, j].Imaginary, 0.0f, 1e-3f);
                    }
                    else
                    {
                        Assert.AreEqual(matrixQсtQ[i, j].Real, 0.0f, 1e-3f);
                        Assert.AreEqual(matrixQсtQ[i, j].Imaginary, 0.0f, 1e-3f);
                    }
                }
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random vector (Ax=b).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVector([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var vectorb = MatrixLoader.GenerateRandomDenseVector(order);
            var resultx = factorGramSchmidt.Solve(vectorb);

            Assert.AreEqual(matrixA.ColumnCount, resultx.Count);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < order; i++)
            {
                Assert.AreEqual(vectorb[i].Real, matrixBReconstruct[i].Real, 1e-3f);
                Assert.AreEqual(vectorb[i].Imaginary, matrixBReconstruct[i].Imaginary, 1e-3f);
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random matrix (AX=B).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var matrixB = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixX = factorGramSchmidt.Solve(matrixB);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, matrixX.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(matrixB.ColumnCount, matrixX.ColumnCount);

            var matrixBReconstruct = matrixA * matrixX;

            // Check the reconstruction.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixB[i, j].Real, matrixBReconstruct[i, j].Real, 1e-3f);
                    Assert.AreEqual(matrixB[i, j].Imaginary, matrixBReconstruct[i, j].Imaginary, 1e-3f);
                }
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve for a random vector into a result vector.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVectorWhenResultVectorGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();
            var vectorb = MatrixLoader.GenerateRandomDenseVector(order);
            var vectorbCopy = vectorb.Clone();
            var resultx = new DenseVector(order);
            factorGramSchmidt.Solve(vectorb, resultx);

            Assert.AreEqual(vectorb.Count, resultx.Count);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < vectorb.Count; i++)
            {
                Assert.AreEqual(vectorb[i].Real, matrixBReconstruct[i].Real, 1e-3f);
                Assert.AreEqual(vectorb[i].Imaginary, matrixBReconstruct[i].Imaginary, 1e-3f);
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }

            // Make sure b didn't change.
            for (var i = 0; i < vectorb.Count; i++)
            {
                Assert.AreEqual(vectorbCopy[i], vectorb[i]);
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random matrix (AX=B) into a result matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrixWhenResultMatrixGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var matrixB = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixBCopy = matrixB.Clone();

            var matrixX = new DenseMatrix(order, order);
            factorGramSchmidt.Solve(matrixB, matrixX);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, matrixX.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(matrixB.ColumnCount, matrixX.ColumnCount);

            var matrixBReconstruct = matrixA * matrixX;

            // Check the reconstruction.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixB[i, j].Real, matrixBReconstruct[i, j].Real, 1e-3f);
                    Assert.AreEqual(matrixB[i, j].Imaginary, matrixBReconstruct[i, j].Imaginary, 1e-3f);
                }
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }

            // Make sure B didn't change.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixBCopy[i, j], matrixB[i, j]);
                }
            }
        }
    }
}