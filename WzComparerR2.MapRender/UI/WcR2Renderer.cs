﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Renderers;
using WzComparerR2.Rendering;

namespace WzComparerR2.MapRender.UI
{
    /// <summary>
    /// Implements Mono Game renderer.
    /// </summary>
    public class WcR2Renderer : Renderer, IDisposable
    {
        /// <summary>
        /// The graphics device
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        public GraphicsDevice GraphicsDevice
        {
            get;
            private set;
        }

        private RasterizerState clippingRasterizeState = new RasterizerState { ScissorTestEnable = true, CullMode = CullMode.None };
        private RasterizerState previousState;
        private SpriteBatchEx spriteBatch;
        private D2DRenderer d2dRenderer;

        private bool isClipped;
        private Stack<Rectangle> clipRectanges;

        private Stack<Effect> activeEffects;
        private Effect currentActiveEffect;

        private BasicEffect basicEffect;
        private RasterizerState rasterizeStateGeometry = new RasterizerState { ScissorTestEnable = false, CullMode = CullMode.None, FillMode = FillMode.Solid };
        //private bool isSpriteRenderInProgress;
        private DrawState currState;

        /// <summary>
        /// Gets a value indicating whether is full screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if is full screen; otherwise, <c>false</c>.
        /// </value>
        public override bool IsFullScreen
        {
            get { return GraphicsDevice.PresentationParameters.IsFullScreen; }
        }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public Matrix Projection
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcR2Renderer" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="nativeScreenWidth">Width of the native screen.</param>
        /// <param name="nativeScreenHeight">Height of the native screen.</param>
        public WcR2Renderer(GraphicsDevice graphicsDevice, int nativeScreenWidth, int nativeScreenHeight)
        {
            spriteBatch = new SpriteBatchEx(graphicsDevice);
            this.d2dRenderer = new D2DRenderer(graphicsDevice);
            GraphicsDevice = graphicsDevice;

            if (graphicsDevice.PresentationParameters.IsFullScreen)
            {
                NativeScreenWidth = nativeScreenWidth;
                NativeScreenHeight = nativeScreenHeight;
            }
            else
            {
                NativeScreenWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
                NativeScreenHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
            }

            clipRectanges = new Stack<Rectangle>();
            activeEffects = new Stack<Effect>();
        }

        /// <summary>
        /// Begins the rendering
        /// </summary>
        public override void Begin()
        {
            Begin(null);
        }

        private void UpdateCurrentEffect(EffectBase effect)
        {
            Effect effectInstance = effect != null ? effect.GetNativeEffect() as Effect : null;
            if (effectInstance != null)
            {
                if (currentActiveEffect != null)
                {
                    activeEffects.Push(currentActiveEffect);
                }

                currentActiveEffect = effectInstance;
            }

            if (currentActiveEffect == null && activeEffects.Count > 0)
            {
                currentActiveEffect = activeEffects.Pop();
            }
        }

        /// <summary>
        /// Draws the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="renderSize">Size of the render.</param>
        /// <param name="color">The color.</param>
        /// <param name="centerOrigin">if set to <c>true</c> [center origin].</param>
        public override void Draw(TextureBase texture, PointF position, Size renderSize, ColorW color, bool centerOrigin)
        {
            Rectangle testRectangle;
            testRectangle.X = (int)position.X;
            testRectangle.Y = (int)position.Y;
            testRectangle.Width = (int)renderSize.Width;
            testRectangle.Height = (int)renderSize.Height;
            if (isClipped && !this.GraphicsDevice.ScissorRectangle.Intersects(testRectangle))
            {
                return;
            }

            this.Prepare(DrawState.Sprite);
            Color vecColor = new Color();
            vecColor.PackedValue = color.PackedValue;
            Texture2D native = texture.GetNativeTexture() as Texture2D;
            spriteBatch.Draw(native, testRectangle, vecColor);
        }

        /// <summary>
        /// Draws the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="renderSize">Size of the render.</param>
        /// <param name="color">The color.</param>
        /// <param name="source">The source.</param>
        /// <param name="centerOrigin">if set to <c>true</c> [center origin].</param>
        public override void Draw(TextureBase texture, PointF position, Size renderSize, ColorW color, Rect source, bool centerOrigin)
        {
            Rectangle testRectangle;
            testRectangle.X = (int)position.X;
            testRectangle.Y = (int)position.Y;
            testRectangle.Width = (int)renderSize.Width;
            testRectangle.Height = (int)renderSize.Height;
            if (isClipped && !this.GraphicsDevice.ScissorRectangle.Intersects(testRectangle))
            {
                return;
            }

            this.Prepare(DrawState.Sprite);
            Rectangle sourceRect;
            sourceRect.X = (int)source.X;
            sourceRect.Y = (int)source.Y;
            sourceRect.Width = (int)source.Width;
            sourceRect.Height = (int)source.Height;
            Color vecColor = new Color();
            vecColor.PackedValue = color.PackedValue;
            Texture2D native = texture.GetNativeTexture() as Texture2D;
            spriteBatch.Draw(native, testRectangle, sourceRect, vecColor, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Ends rendering
        /// </summary>
        public override void End(bool endEffect = false)
        {
            this.Flush();
            isClipped = false;
            if (endEffect)
            {
                currentActiveEffect = null;
            }
            else
            {
                activeEffects.Push(currentActiveEffect);
                currentActiveEffect = null;
            }
        }

        /// <summary>
        /// Begins the clipped.
        /// </summary>
        /// <param name="clipRect">The clip rect.</param>
        public override void BeginClipped(Rect clipRect)
        {
            BeginClipped(clipRect, null);
        }

        /// <summary>
        /// Begins the clipped rendering with custom effect
        /// </summary>
        /// <param name="clipRect">The clip rect.</param>
        /// <param name="effect">The effect.</param>
        public override void BeginClipped(Rect clipRect, EffectBase effect)
        {
            Rectangle clipRectangle;
            clipRectangle.X = (int)clipRect.X;
            clipRectangle.Y = (int)clipRect.Y;
            clipRectangle.Width = (int)clipRect.Width;
            clipRectangle.Height = (int)clipRect.Height;

            UpdateCurrentEffect(effect);

            BeginClipped(clipRectangle);
        }

        /// <summary>
        /// Ends the clipped drawing
        /// </summary>
        public override void EndClipped(bool endEffect = false)
        {
            this.Flush();
            isClipped = false;

            if (endEffect)
            {
                currentActiveEffect = null;
            }
            else
            {
                activeEffects.Push(currentActiveEffect);
                currentActiveEffect = null;
            }

            clipRectanges.Pop();
        }

        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <returns></returns>
        public override Rect GetViewport()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            return new Rect(viewport.X, viewport.Y, viewport.Width, viewport.Height);
        }

        /// <summary>
        /// Creates the texture.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="mipmap">if set to <c>true</c> [mipmap].</param>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <returns></returns>
        public override TextureBase CreateTexture(int width, int height, bool mipmap, bool dynamic)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            Texture2D native = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            MonoGameTexture texture = new MonoGameTexture(native);
            return texture;
        }

        /// <summary>
        /// Resets the size of the native. Sets NativeScreenWidth and NativeScreenHeight based on active back buffer
        /// </summary>
        public override void ResetNativeSize()
        {
            NativeScreenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            NativeScreenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        /// <summary>
        /// Draws the color of the geometry.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="depth">The depth.</param>
        public override void DrawGeometryColor(GeometryBuffer buffer, PointF position, ColorW color, float opacity, float depth)
        {
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(GraphicsDevice);
            }

            basicEffect.Alpha = color.A / (float)byte.MaxValue * opacity;
            //color = color * effect.Alpha;
            basicEffect.DiffuseColor = new Vector3(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue);
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;

            DrawGeometry(buffer, position, depth);
        }

        /// <summary>
        /// Draws the geometry texture.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="depth">The depth.</param>
        public override void DrawGeometryTexture(GeometryBuffer buffer, PointF position, TextureBase texture, float opacity, float depth)
        {
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(GraphicsDevice);
            }

            basicEffect.Alpha = opacity;
            basicEffect.DiffuseColor = new Vector3(1, 1, 1);
            basicEffect.Texture = texture.GetNativeTexture() as Texture2D;
            basicEffect.VertexColorEnabled = false;
            basicEffect.TextureEnabled = true;

            DrawGeometry(buffer, position, depth);
        }

        /// <summary>
        /// Determines whether the specified rectangle is outside of clip bounds
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="renderSize">Size of the render.</param>
        /// <returns></returns>
        public override bool IsClipped(PointF position, Size renderSize)
        {
            if (isClipped)
            {
                Rectangle testRectangle;
                testRectangle.X = (int)position.X;
                testRectangle.Y = (int)position.Y;
                testRectangle.Width = (int)renderSize.Width;
                testRectangle.Height = (int)renderSize.Height;

                if (!this.GraphicsDevice.ScissorRectangle.Intersects(testRectangle))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the effect.
        /// </summary>
        /// <param name="nativeEffect">The native effect.</param>
        /// <returns></returns> 
        public override EffectBase CreateEffect(object nativeEffect)
        {
            return new MonoGameEffect(nativeEffect);
        }

        /// <summary>
        /// Gets the SDF font effect.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override EffectBase GetSDFFontEffect()
        {
            throw new NotImplementedException();
        }

        public override FontBase CreateFont(object nativeFont)
        {
            return new WcR2Font(nativeFont);
        }

        public override TextureBase CreateTexture(object nativeTexture)
        {
            if (nativeTexture is Texture2D)
            {
                return new MonoGameTexture(nativeTexture);
            }
            else
            {
                throw new ArgumentException("nativeTexture is not Texture2D.");
            }
        }

        public override void DrawText(FontBase font, string text, PointF position, Size renderSize, ColorW color, PointF scale, float depth)
        {
            var wcR2Font = (font.GetNativeFont() as IWcR2Font)?.BaseFont;
            if (wcR2Font != null)
            {
                if (wcR2Font is XnaFont)
                {
                    //snap pixels
                    position.X = (float)Math.Round(position.X);
                    position.Y = (float)Math.Round(position.Y);

                    Prepare(DrawState.Sprite);
                    spriteBatch.DrawStringEx((XnaFont)wcR2Font,
                        text,
                        new Vector2(position.X, position.Y),
                        new Vector2(renderSize.Width, renderSize.Height),
                        new Color(color.R, color.G, color.B, color.A));
                }
                else if (wcR2Font is D2DFont)
                {
                    Prepare(DrawState.D2D);
                    d2dRenderer.DrawString((D2DFont)wcR2Font,
                        text,
                        new Vector2(position.X, position.Y),
                        new Vector2(renderSize.Width, renderSize.Height),
                        new Color(color.R, color.G, color.B, color.A));
                }
            }
        }

        /// <summary>
        /// Begins the rendering with custom effect
        /// </summary>
        /// <param name="effect">The effect.</param>
        public override void Begin(EffectBase effect)
        {
            isClipped = false;
            UpdateCurrentEffect(effect);
            if (previousState != null)
            {
                GraphicsDevice.RasterizerState = previousState;
                previousState = null;
            }

            if (clipRectanges.Count > 0)
            {
                Rectangle previousClip = clipRectanges.Pop();
                BeginClipped(previousClip);
            }
            else
            {
                currState = DrawState.None;
            }
        }

        /// <summary>
        /// Begins the clipped.
        /// </summary>
        /// <param name="clipRect">The clip rect.</param>
        private void BeginClipped(Rectangle clipRect)
        {
            isClipped = true;
            currState = DrawState.None;

            if (clipRectanges.Count > 0)
            {
                Rectangle previousClip = clipRectanges.Peek();
                if (previousClip.Intersects(clipRect))
                {
                    clipRect = Rectangle.Intersect(previousClip, clipRect);
                }
                else
                {
                    clipRect = previousClip;
                }
            }

            GraphicsDevice.ScissorRectangle = clipRect;
            previousState = spriteBatch.GraphicsDevice.RasterizerState;
            clipRectanges.Push(clipRect);
        }

        /// <summary>
        /// Creates the geometry buffer.
        /// </summary>
        /// <returns></returns>
        public override GeometryBuffer CreateGeometryBuffer()
        {
            return new WcR2GeometryBuffer(this.GraphicsDevice);
        }

        private void DrawGeometry(GeometryBuffer buffer, PointF position, float depth)
        {
            this.Prepare(DrawState.Geometry);

            WcR2GeometryBuffer wcR2Buffer = buffer as WcR2GeometryBuffer;
            GraphicsDevice device = GraphicsDevice;

            RasterizerState rasState = device.RasterizerState;
            BlendState blendState = device.BlendState;
            DepthStencilState stencilState = device.DepthStencilState;

            device.BlendState = BlendState.NonPremultiplied;
            device.DepthStencilState = DepthStencilState.DepthRead;

            if (isClipped)
            {
                device.RasterizerState = clippingRasterizeState;
            }
            else
            {
                device.RasterizerState = rasterizeStateGeometry;
            }

            basicEffect.World = Matrix.CreateTranslation(position.X, position.Y, depth);
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)device.Viewport.Width, (float)device.Viewport.Height, 0, 1.0f, 1000.0f);

            device.SetVertexBuffer(wcR2Buffer.VertexBuffer);
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                switch (buffer.PrimitiveType)
                {
                    case GeometryPrimitiveType.TriangleList:
                        device.DrawPrimitives(PrimitiveType.TriangleList, 0, wcR2Buffer.PrimitiveCount);
                        break;
                    case GeometryPrimitiveType.TriangleStrip:
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, wcR2Buffer.PrimitiveCount);
                        break;
                    case GeometryPrimitiveType.LineList:
                        device.DrawPrimitives(PrimitiveType.LineList, 0, wcR2Buffer.PrimitiveCount);
                        break;
                    case GeometryPrimitiveType.LineStrip:
                        device.DrawPrimitives(PrimitiveType.LineStrip, 0, wcR2Buffer.PrimitiveCount);
                        break;
                    default:
                        break;
                }
            }

            device.DepthStencilState = stencilState;
            device.BlendState = blendState;
            device.RasterizerState = rasState;
        }

        private void Prepare(DrawState nextState)
        {
            if (this.currState == nextState)
            {
                return;
            }

            Flush();

            switch (nextState)
            {
                case DrawState.None: break;
                case DrawState.Geometry: break;
                case DrawState.Sprite:
                    RasterizerState rasterizer = isClipped ? this.clippingRasterizeState : null;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, effect: currentActiveEffect);
                    break;

                case DrawState.D2D:
                    d2dRenderer.Begin();
                    if (isClipped)
                    {
                        d2dRenderer.PushClip(this.GraphicsDevice.ScissorRectangle);
                    }
                    break;
            }

            this.currState = nextState;
        }

        private void Flush()
        {
            switch (this.currState)
            {
                case DrawState.None: break;
                case DrawState.Geometry: break;
                case DrawState.Sprite:
                    this.spriteBatch.End();
                    break;

                case DrawState.D2D:
                    if (isClipped)
                    {
                        this.d2dRenderer.PopClip();
                    }
                    this.d2dRenderer.End();
                    break;
            }
        }

        public void Dispose()
        {
            this.activeEffects.Clear();

            this.clippingRasterizeState?.Dispose();
            this.rasterizeStateGeometry?.Dispose();
            this.spriteBatch?.Dispose();
            this.basicEffect?.Dispose();
        }

        enum DrawState
        {
            None = 0,
            Geometry = 1,
            Sprite = 2,
            D2D = 3
        }
    }

    public class WcR2GeometryBuffer : GeometryBuffer
    {
        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        public VertexBuffer VertexBuffer { get; set; }

        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcR2GeometryBuffer"/> class.
        /// </summary>
        public WcR2GeometryBuffer(GraphicsDevice device)
            : base()
        {
            this.graphicsDevice = device;
        }

        /// <summary>
        /// Fills the color type buffer (VertexPositionColor)
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="primitiveType">Type of the primitive.</param>
        public override void FillColor(List<PointF> points, GeometryPrimitiveType primitiveType)
        {
            SetPrimitiveCount(primitiveType, points.Count);

            VertexPositionColor[] vertex = new VertexPositionColor[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertex[i] = new VertexPositionColor(new Vector3(points[i].X, points[i].Y, 0), Color.White);
            }

            VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, vertex.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionColor>(vertex);
        }

        private void SetPrimitiveCount(GeometryPrimitiveType primitiveType, int pointCount)
        {
            PrimitiveType = primitiveType;
            switch (primitiveType)
            {
                case GeometryPrimitiveType.TriangleList:
                    PrimitiveCount = pointCount / 3;
                    break;
                case GeometryPrimitiveType.TriangleStrip:
                    PrimitiveCount = pointCount - 2;
                    break;
                case GeometryPrimitiveType.LineList:
                    PrimitiveCount = pointCount / 2;
                    break;
                case GeometryPrimitiveType.LineStrip:
                    PrimitiveCount = pointCount - 1;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Fills the texture type buffer (VertexPositionTexture)
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="destinationSize">Size of the destination.</param>
        /// <param name="sourceRect">The source rect.</param>
        /// <param name="primitiveType">Type of the primitive.</param>
        public override void FillTexture(List<PointF> points, Size destinationSize, Rect sourceRect, GeometryPrimitiveType primitiveType)
        {
            SetPrimitiveCount(primitiveType, points.Count);

            VertexPositionTexture[] vertex = new VertexPositionTexture[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 uv = new Vector2(sourceRect.X + (points[i].X / destinationSize.Width) * sourceRect.Width,
                                         sourceRect.Y + (points[i].Y / destinationSize.Height) * sourceRect.Height);
                vertex[i] = new VertexPositionTexture(new Vector3(points[i].X, points[i].Y, 0), uv);
            }

            VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertex.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionTexture>(vertex);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (VertexBuffer != null && !VertexBuffer.IsDisposed)
            {
                VertexBuffer.Dispose();
            }
        }
    }
}
