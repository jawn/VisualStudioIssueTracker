﻿using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitHubTracker
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("GitHubGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(GitHubTag))]
    internal sealed class GitHubGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new GitHubGlyphFactory();
        }

        private class GitHubGlyphFactory : IGlyphFactory
        {
            const double m_glyphSize = 16.0;

            public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
            {
                var gitHubTag = tag as GitHubTag;

                // Ensure we can draw a glyph for this marker.
                if (gitHubTag == null)
                {
                    return null;
                }

                var border = new Border
                {
                    Height = m_glyphSize,
                    Width = m_glyphSize,
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.Black
                };

                var rectangle = new Rectangle
                {
                    Height = m_glyphSize,
                    Width = m_glyphSize,
                };

                border.Child = rectangle;

                gitHubTag.Update(t =>
                {
                    rectangle.Dispatcher.Invoke(() =>
                    {
                        switch (t)
                        {
                            case IssueStatus.Closed:
                                rectangle.Fill = Brushes.Green;
                                break;
                            case IssueStatus.Open:
                                rectangle.Fill = Brushes.Red;
                                break;
                            case IssueStatus.Unavailable:
                                rectangle.Fill = Brushes.CadetBlue;
                                break;
                            case IssueStatus.RateLimited:
                                rectangle.Fill = Brushes.AliceBlue;
                                break;
                        }
                    });
                });

                return border;
            }
        }
    }
}
