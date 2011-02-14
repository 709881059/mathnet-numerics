﻿// <copyright file="UserEvdTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex.Factorization
{
    using System;
    using System.Numerics;
    using LinearAlgebra.Complex.Factorization;
    using LinearAlgebra.Generic.Factorization;
    using NUnit.Framework;

    /// <summary>
    /// Eigenvalues factorization tests for an user matrix.
    /// </summary>
    public class UserEvdTests
    {
        /// <summary>
        /// Constructor <c>null</c> throws <c>ArgumentNullException</c>.
        /// </summary>
        [Test]
        public void ConstructorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new UserEvd(null));
        }

        /// <summary>
        /// Can factorize identity matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanFactorizeIdentity([Values(1, 10, 100)] int order)
        {
            var matrixI = UserDefinedMatrix.Identity(order);
            var factorEvd = matrixI.Evd();

            Assert.AreEqual(matrixI.RowCount, factorEvd.EigenVectors().RowCount);
            Assert.AreEqual(matrixI.RowCount, factorEvd.EigenVectors().ColumnCount);

            Assert.AreEqual(matrixI.ColumnCount, factorEvd.D().RowCount);
            Assert.AreEqual(matrixI.ColumnCount, factorEvd.D().ColumnCount);

            for (var i = 0; i < factorEvd.EigenValues().Count; i++)
            {
                Assert.AreEqual(Complex.One, factorEvd.EigenValues()[i]);
            }
        }

        /// <summary>
        /// Can factorize a random square matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanFactorizeRandomMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomUserDefinedMatrix(order, order);
            var factorEvd = matrixA.Evd();

            Assert.AreEqual(order, factorEvd.EigenVectors().RowCount);
            Assert.AreEqual(order, factorEvd.EigenVectors().ColumnCount);

            Assert.AreEqual(order, factorEvd.D().RowCount);
            Assert.AreEqual(order, factorEvd.D().ColumnCount);

            // Make sure the A*V = λ*V 
            var matrixAv = matrixA * factorEvd.EigenVectors();
            var matrixLv = factorEvd.EigenVectors() * factorEvd.D();

            for (var i = 0; i < matrixAv.RowCount; i++)
            {
                for (var j = 0; j < matrixAv.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(matrixAv[i, j], matrixLv[i, j], 7);
                }
            }
        }

        /// <summary>
        /// Can factorize a symmetric random square matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanFactorizeRandomSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomPositiveDefiniteHermitianUserDefinedMatrix(order);
            var factorEvd = matrixA.Evd();

            Assert.AreEqual(order, factorEvd.EigenVectors().RowCount);
            Assert.AreEqual(order, factorEvd.EigenVectors().ColumnCount);

            Assert.AreEqual(order, factorEvd.D().RowCount);
            Assert.AreEqual(order, factorEvd.D().ColumnCount);

            // Make sure the A = V*λ*VT 
            var matrix = factorEvd.EigenVectors() * factorEvd.D() * factorEvd.EigenVectors().ConjugateTranspose();

            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(matrix[i, j], matrixA[i, j], 7);
                }
            }
        }

        /// <summary>
        /// Can check rank of square matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanCheckRankSquare([Values(10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomUserDefinedMatrix(order, order);
            var factorEvd = matrixA.Evd();

            Assert.AreEqual(factorEvd.Rank, order);
        }

        /// <summary>
        /// Can check rank of square singular matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanCheckRankOfSquareSingular([Values(10, 50, 100)] int order)
        {
            var matrixA = new UserDefinedMatrix(order, order);
            matrixA[0, 0] = 1;
            matrixA[order - 1, order - 1] = 1;
            for (var i = 1; i < order - 1; i++)
            {
                matrixA[i, i - 1] = 1;
                matrixA[i, i + 1] = 1;
                matrixA[i - 1, i] = 1;
                matrixA[i + 1, i] = 1;
            }

            var factorEvd = matrixA.Evd();

            Assert.AreEqual(factorEvd.Determinant, Complex.Zero);
            Assert.AreEqual(factorEvd.Rank, order - 1);
        }

        /// <summary>
        /// Identity determinant is one.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void IdentityDeterminantIsOne([Values(1, 10, 100)] int order)
        {
            var matrixI = UserDefinedMatrix.Identity(order);
            var factorEvd = matrixI.Evd();
            Assert.AreEqual(Complex.One, factorEvd.Determinant);
        }

        /// <summary>
        /// Can solve a system of linear equations for a random vector and symmetric matrix (Ax=b).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVectorAndSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomPositiveDefiniteHermitianUserDefinedMatrix(order);
            var matrixACopy = matrixA.Clone();
            var factorEvd = matrixA.Evd();

            var vectorb = MatrixLoader.GenerateRandomUserDefinedVector(order);
            var resultx = factorEvd.Solve(vectorb);

            Assert.AreEqual(matrixA.ColumnCount, resultx.Count);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < vectorb.Count; i++)
            {
                AssertHelpers.AlmostEqual(vectorb[i], matrixBReconstruct[i], 7);
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
        /// Can solve a system of linear equations for a random matrix and symmetric matrix (AX=B).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrixAndSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomPositiveDefiniteHermitianUserDefinedMatrix(order);
            var matrixACopy = matrixA.Clone();
            var factorEvd = matrixA.Evd();

            var matrixB = MatrixLoader.GenerateRandomUserDefinedMatrix(order, order);
            var matrixX = factorEvd.Solve(matrixB);

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
                    AssertHelpers.AlmostEqual(matrixB[i, j], matrixBReconstruct[i, j], 7);
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
        /// Can solve a system of linear equations for a random vector and symmetric matrix (Ax=b) into a result matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVectorAndSymmetricMatrixWhenResultVectorGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomPositiveDefiniteHermitianUserDefinedMatrix(order);
            var matrixACopy = matrixA.Clone();
            var factorEvd = matrixA.Evd();
            var vectorb = MatrixLoader.GenerateRandomUserDefinedVector(order);
            var vectorbCopy = vectorb.Clone();
            var resultx = new UserDefinedVector(order);
            factorEvd.Solve(vectorb, resultx);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < vectorb.Count; i++)
            {
                AssertHelpers.AlmostEqual(vectorb[i], matrixBReconstruct[i], 7);
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
        /// Can solve a system of linear equations for a random matrix and symmetric matrix (AX=B) into result matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrixAndSymmetricMatrixWhenResultMatrixGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var matrixA = MatrixLoader.GenerateRandomPositiveDefiniteHermitianUserDefinedMatrix(order);
            var matrixACopy = matrixA.Clone();
            var factorEvd = matrixA.Evd();

            var matrixB = MatrixLoader.GenerateRandomUserDefinedMatrix(order, order);
            var matrixBCopy = matrixB.Clone();

            var matrixX = new UserDefinedMatrix(order, order);
            factorEvd.Solve(matrixB, matrixX);

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
                    AssertHelpers.AlmostEqual(matrixB[i, j], matrixBReconstruct[i, j], 7);
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