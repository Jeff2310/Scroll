using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Scroll
{
    public class Region
    {
        public List<Coordinate> Vertices;
        public Coordinate _min, _max;

        public Region()
        {
            Vertices = new List<Coordinate>();
            _min = new Coordinate {Latitude = 10000.0, Longtitude = 10000.0};
            _max = new Coordinate {Latitude = -1.0, Longtitude = -1.0};
        }

        public void AddVertex(Coordinate vertex)
        {
            Vertices.Add(vertex);
            _min.Longtitude = Math.Min(_min.Longtitude, vertex.Longtitude);
            _min.Latitude = Math.Min(_min.Latitude, vertex.Latitude);
            _max.Longtitude = Math.Max(_max.Longtitude, vertex.Longtitude);
            _max.Latitude = Math.Max(_max.Latitude, vertex.Latitude);
        }

        public void GetBoundingBox(out Coordinate min, out Coordinate max)
        {
            min = _min;
            max = _max;
        }
    }
}