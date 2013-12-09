using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Common;

namespace DyedSudoku
{
	public class GameFieldViewModel
	{
		private enum EMode
		{
			Normal,
			Dialog,
			Win,
			Lose
		}

		private readonly FPSCounter fpsCounter = new FPSCounter();

		public RectangleF Frame { get; private set; }

		private IndexPair SelectedPair { get; set; }

		private GameFieldModel Model { get; set; }

		private DialogViewModel DialogViewModel { get; set; }

		private EMode Mode { get; set; }

		public bool IsEnd
		{
			get { return Mode == EMode.Win || Mode == EMode.Lose; }
		}

		public bool NeedRefresh
		{
			get { return Model != null && Model.IsInitializing || Mode == EMode.Dialog && DialogViewModel != null && DialogViewModel.IsInvisible; }
		}

		private float cellHeight;
		private float cellWidth;
		private float blockHeight;
		private float blockWidth;
		private CGContext context;
		private const float cellContentLeft = 10;
		private const float cellContentBottom = 8;
		private const float dialogResultWidth = 250;
		private const float dialogResultHeight = 150;
		private const float dialogBorder = 3;
		private const float dialogContentBottom = 30;
		private const float dialogOffset = 1;

		public GameFieldViewModel(RectangleF frame)
		{
			InitModel();
			InitCellSizes(frame);
		}

		public void InitModel()
		{
			Model = new GameFieldModel();
			Mode = EMode.Normal;
		}

		public void Cancel()
		{
			Model.Cancel();
		}

		public void InitCellSizes(RectangleF frame)
		{
			Frame = frame;

			cellHeight = Frame.Height / Model.CellLineCount;
			cellWidth = Frame.Width / Model.CellLineCount;
			blockHeight = Frame.Height / Model.BlockLineCount;
			blockWidth = Frame.Width / Model.BlockLineCount;
		}

		public void Draw(CGContext ctx)
		{            
			fpsCounter.Tick();

			SetCurrentContext(ctx);

			if (Model.IsInitializing)
			{
				DrawBackground();
				DrawCellsBackground();
			}
			else
			{
				DrawSelected();
				DrawModel();
			}

			DrawLines();
			DrawDialogs();
			//DrawFPS();
		}

		private void SetCurrentContext(CGContext ctx)
		{
			context = ctx;

			context.SetDefaultCTMSettings(Frame.Height);
			context.SetDefaultTextSettings();
		}

		private void DrawBackground()
		{
			context.SetFillEmptyItemColor();
			context.AddRect(new RectangleF(0, 0, Frame.Width, Frame.Height));
			context.DrawPath(CGPathDrawingMode.Fill);
		}

		private void DrawCellsBackground()
		{
			foreach (var pair in Model.GetAllPairs())
			{
				if (Model.IsItemEmpty(pair))
					continue;

				if (Model.IsSelectedPair(pair))
					context.SetFillSelectedItemColor();
				else if (Model.GetItemVisible(pair))
					context.SetFillVisibleItemColor();
				else
					context.SetFillInitializedItemColor();

				FillCellBackground(pair);
			}
		}

		private void FillCellBackground(IndexPair pair)
		{
			context.AddRect(new RectangleF(pair.X * cellWidth, pair.Y * cellHeight, cellWidth, cellHeight));
			context.DrawPath(CGPathDrawingMode.Fill);
		}

		private void DrawLines()
		{
			context.SetDefaultLineSettings();

			DrawCellLines();
			DrawBlockLines();
		}

		private void DrawCellLines()
		{
			DrawGrid(cellHeight, cellWidth, 1);
		}

		private void DrawBlockLines()
		{
			DrawGrid(blockHeight, blockWidth, 2);
		}

		private void DrawGrid(float height, float width, float lineWidth)
		{
			context.SetLineWidth(lineWidth);

			for (float i = 0; i <= Frame.Height; i += height)
				context.AddLines(new[] { new PointF(0, i), new PointF(Frame.Width, i) });

			for (float i = 0; i <= Frame.Width; i += width)
				context.AddLines(new[] { new PointF(i, 0), new PointF(i, Frame.Height) });

			context.DrawPath(CGPathDrawingMode.Stroke);
		}

		private void DrawModel()
		{
			context.SetDefaultTextSettings();

			foreach (var pair in Model.GetAllPairs())
			{
				if (!Model.GetItemVisible(pair))
					continue;

				context.DrawText(Model.GetItemNumber(pair).ToString(), pair.X * cellWidth + cellContentLeft, pair.Y * cellHeight + cellContentBottom);
			}
		}

		private void DrawFPS()
		{
			context.SetDefaultInfoTextSettings();

			context.DrawInfoText(DateTime.Now.ToLongTimeString(), Frame.Width - 100, Frame.Height - 20);
			context.DrawInfoText(fpsCounter.GetFPS().ToString(), Frame.Width - 30, Frame.Height - 20);
		}

		private void DrawSelected()
		{
			if (SelectedPair == null)
				return;

			context.SetFillSelectedItemColor();
			FillCellBackground(SelectedPair);
		}

		public void UpdateByTap(PointF point)
		{
			if (Model.IsInitializing)
				return;

			var ctmPoint = new PointF(point.X, Frame.Height - point.Y);

			if (Mode == EMode.Normal)
			{
				var x = (int)(ctmPoint.X / cellWidth);
				var y = (int)(ctmPoint.Y / cellHeight);
				var pair = new IndexPair(x, y);

				if (Model.GetItemVisible(pair))
					return;

				SelectedPair = pair;

				DialogViewModel = new DialogViewModel(GetDialogFrame(ctmPoint), dialogBorder);

				Mode = EMode.Dialog;

				return;
			}

			if (Mode == EMode.Dialog)
			{
				if (DialogViewModel == null || DialogViewModel.IsInvisible)
					return;

				if (DialogViewModel.IsInside(ctmPoint))
				{
					Model.SetItemVisible(SelectedPair, true);

					if (DialogViewModel.GetNumber(ctmPoint) != Model.GetItemNumber(SelectedPair))
					{
						Mode = EMode.Lose;
					}
					else if (Model.IsAllItemsVisible)
					{
						Mode = EMode.Win;
					}
					else
					{
						Mode = EMode.Normal;
					}
				}
				else
				{
					Mode = EMode.Normal;
				}

				SelectedPair = null;
				DialogViewModel = null;

				return;
			}
		}

		public void UpdateByOffboardTap()
		{
			if (Mode != EMode.Dialog)
				return;

			Mode = EMode.Normal;
			SelectedPair = null;
			DialogViewModel = null;
		}

		private RectangleF GetDialogFrame(PointF point)
		{
			var dialogWidth = 4 * cellWidth;
			var dialogHeight = 4 * cellHeight;

			var left = point.X - dialogWidth / 2;
			var bottom = point.Y - dialogHeight / 2;

			if (left < dialogOffset)
				left = dialogOffset;

			if (bottom < dialogOffset)
				bottom = dialogOffset;

			if (left + dialogWidth > Frame.Width - dialogOffset)
				left = Frame.Width - dialogWidth - dialogOffset;

			if (bottom + dialogHeight > Frame.Height - dialogOffset)
				bottom = Frame.Height - dialogHeight - dialogOffset;

			return new RectangleF(left, bottom, dialogWidth, dialogHeight);
		}

		public void DrawDialogs()
		{
			switch (Mode)
			{
				case EMode.Normal:
					break;
				case EMode.Dialog:
					DrawDialog();
					break;
				case EMode.Win:
				case EMode.Lose:
					DrawResultInfo();
					break;
				default:
					throw new ArgumentException("Mode");
			}
		}

		public void DrawDialog()
		{
			if (DialogViewModel == null)
				return;

			DialogViewModel.Draw(context);
		}

		public void DrawResultInfo()
		{
			var centerHeight = Frame.Height / 2;
			var centerWigth = Frame.Width / 2;
			var left = centerWigth - dialogResultWidth / 2;
			var bottom = centerHeight - dialogResultHeight / 2;

			context.SetFillDialogBorderColor();

			context.AddRect(
				new RectangleF(left, 
					bottom, 
					dialogResultWidth, 
					dialogResultHeight));

			context.DrawPath(CGPathDrawingMode.Fill);

			if (Mode == EMode.Win)
				context.SetFillWinInfoBackgroundColor();
			else
				context.SetFillLoseInfoBackgroundColor();

			context.AddRect(
				new RectangleF(left + dialogBorder, 
					bottom + dialogBorder, 
					dialogResultWidth - 2 * dialogBorder, 
					dialogResultHeight - 2 * dialogBorder));

			context.DrawPath(CGPathDrawingMode.Fill);

			context.SetDefaultTextSettings();
			context.DrawResultText(Mode == EMode.Win ? "Win" : "Lose", centerWigth, centerHeight - dialogContentBottom, true);
		}
	}
}