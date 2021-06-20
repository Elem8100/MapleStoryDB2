﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EKTexture = EmptyKeys.UserInterface.Media.TextureBase;

namespace WzComparerR2.MapRender.UI
{
    public class NineFormResource : INinePatchResource<Texture2D>
    {
        /// <summary>
        /// 左上
        /// </summary>
        public Texture2D NW { get; set; }
        /// <summary>
        /// 上。
        /// </summary>
        public Texture2D N { get; set; }
        /// <summary>
        /// 右上。
        /// </summary>
        public Texture2D NE { get; set; }

        /// <summary>
        /// 左。
        /// </summary>
        public Texture2D W { get; set; }
        /// <summary>
        /// 中。
        /// </summary>
        public Texture2D C { get; set; }
        /// <summary>
        /// 右。
        /// </summary>
        public Texture2D E { get; set; }

        /// <summary>
        /// 左下。
        /// </summary>
        public Texture2D SW { get; set; }
        /// <summary>
        /// 下。
        /// </summary>
        public Texture2D S { get; set; }
        /// <summary>
        /// 右下。
        /// </summary>
        public Texture2D SE { get; set; }

        public bool IsFitSize
        {
            get
            {
                return this.NW.Height == this.NE.Height
                    && this.NE.Width == this.SE.Width
                    && this.SE.Height == this.SW.Height
                    && this.SW.Width == this.NW.Width;
            }
        }

        public Point GetSize(Texture2D texture)
        {
            return texture == null ? Point.Zero : new Point(texture.Width, texture.Height);
        }
    }

    public class EKNineFormResource : INinePatchResource<EKTexture>
    {
        /// <summary>
        /// 左上
        /// </summary>
        public EKTexture NW { get; set; }
        /// <summary>
        /// 上。
        /// </summary>
        public EKTexture N { get; set; }
        /// <summary>
        /// 右上。
        /// </summary>
        public EKTexture NE { get; set; }

        /// <summary>
        /// 左。
        /// </summary>
        public EKTexture W { get; set; }
        /// <summary>
        /// 中。
        /// </summary>
        public EKTexture C { get; set; }
        /// <summary>
        /// 右。
        /// </summary>
        public EKTexture E { get; set; }

        /// <summary>
        /// 左下。
        /// </summary>
        public EKTexture SW { get; set; }
        /// <summary>
        /// 下。
        /// </summary>
        public EKTexture S { get; set; }
        /// <summary>
        /// 右下。
        /// </summary>
        public EKTexture SE { get; set; }

        public bool IsFitSize
        {
            get
            {
                return this.NW.Height == this.NE.Height
                    && this.NE.Width == this.SE.Width
                    && this.SE.Height == this.SW.Height
                    && this.SW.Width == this.NW.Width;
            }
        }

        public Point GetSize(EKTexture texture)
        {
            return texture == null ? Point.Zero : new Point(texture.Width, texture.Height);
        }
    }
}
