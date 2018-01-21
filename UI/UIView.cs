using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.UI
{
	public class UIView : BaseElement
	{
		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerView : BaseElement
		{
			public override bool ContainsPoint(Vector2 point) => true;

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 position = Parent.GetDimensions().Position();
				Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
				for (int i = 0; i < Elements.Count; i++)
				{
					UIElement current = Elements[i];
					Vector2 position2 = current.GetDimensions().Position();
					Vector2 dimensions2 = new Vector2(current.GetDimensions().Width, current.GetDimensions().Height);
					if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2)) current.Draw(spriteBatch);
				}
			}
		}

		public List<UIViewElement> items = new List<UIViewElement>();
		internal UIElement innerView = new UIInnerView();

		public Action<SpriteBatch> PreDrawChildren;

		public int Count => items.Count;

		public UIView()
		{
			innerView.Width.Set(0f, 1f);
			innerView.Height.Set(0f, 1f);
			innerView.Center();
			Append(innerView);
			OverflowHidden = true;
		}
		
		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < items.Count; i++) items[i].Update(gameTime);

			base.Update(gameTime);
		}

		public virtual void Add(UIViewElement item)
		{
			item.Recalculate();
			items.Add(item);
			innerView.Append(item);
			UpdateOrder();
			innerView.Recalculate();
		}

		public virtual bool Remove(UIViewElement item)
		{
			innerView.RemoveChild(item);
			UpdateOrder();
			return items.Remove(item);
		}

		public virtual void Clear()
		{
			innerView.RemoveAllChildren();
			items.Clear();
		}

		public override void Recalculate()
		{
			base.Recalculate();

			innerView.Recalculate();
		}

		public override void RecalculateChildren()
		{
			base.RecalculateChildren();

			float left = GetDimensions().Width / 2;
			float top = GetDimensions().Height / 2;

			for (int i = 0; i < items.Count; i++)
			{
				items[i].Left.Pixels = items[i].BasePosition.X + left - items[i].GetDimensions().Width / 2;
				items[i].Top.Pixels = items[i].BasePosition.Y + top - items[i].GetDimensions().Height / 2;
				items[i].Recalculate();
			}
		}

		public void UpdateOrder()
		{
			items.Sort(SortMethod);
		}

		public int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

		public override List<SnapPoint> GetSnapPoints()
		{
			List<SnapPoint> list = new List<SnapPoint>();
			SnapPoint item;
			if (GetSnapPoint(out item)) list.Add(item);
			foreach (UIViewElement current in items) list.AddRange(current.GetSnapPoints());
			return list;
		}

		public Vector2 offset;
		public bool dragging;

		public override void MouseDown(UIMouseEvent evt)
		{
			offset = new Vector2(evt.MousePosition.X - innerView.Left.Pixels, evt.MousePosition.Y - innerView.Top.Pixels);
			dragging = true;
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (dragging)
			{
				Vector2 end = evt.MousePosition;
				dragging = false;

				innerView.Left.Set(end.X - offset.X, 0f);
				innerView.Top.Set(end.Y - offset.Y, 0f);
			}

			Recalculate();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 MousePosition = new Vector2(Main.mouseX, Main.mouseY);
			if (Parent.ContainsPoint(MousePosition)) Main.LocalPlayer.mouseInterface = true;
			if (dragging)
			{
				innerView.Left.Set(MousePosition.X - offset.X, 0f);
				innerView.Top.Set(MousePosition.Y - offset.Y, 0f);
				Recalculate();
			}

			DrawSelf(spriteBatch);

			RasterizerState state = new RasterizerState { ScissorTestEnable = true };
			Rectangle prevRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.End();

			Rectangle scissor = Rectangle.Intersect(GetClippingRectangle(spriteBatch), spriteBatch.GraphicsDevice.ScissorRectangle);

			spriteBatch.GraphicsDevice.ScissorRectangle = scissor;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, state, null, Main.UIScaleMatrix);

			PreDrawChildren?.Invoke(spriteBatch);

			typeof(UIInnerView).InvokeMethod<object>("DrawChildren", new object[] { spriteBatch }, innerView);

			spriteBatch.End();
			spriteBatch.GraphicsDevice.ScissorRectangle = prevRect;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, null, null, Main.UIScaleMatrix);
		}
	}
}