﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;

namespace FaceAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceRectangle[] obamaRect, kimRect;
            FaceLandmarks[] obamaLandmarks, kimLandmarks;
            string obamaFile = "G:\\facedata\\a.jpg";
            string kimFile = "G:\\facedata\\b.jpg";

            runFaceAPI(obamaFile, out obamaRect, out obamaLandmarks);
            runFaceAPI(kimFile, out kimRect, out kimLandmarks);

            PointF[] obamaLandmark = convertLandmarkFormation(ref obamaLandmarks[0], ref obamaRect[0]);
            PointF[] kimLandmark = convertLandmarkFormation(ref kimLandmarks[0], ref kimRect[0]);

            Rectangle obamaRectangle = convertRectangleFormation(obamaRect[0]);
            Rectangle kimRectangle = convertRectangleFormation(kimRect[0]);

            Image<Bgr, byte> obamaFace = new Image<Bgr, byte>(obamaFile).GetSubRect(obamaRectangle);
            Image<Bgr, byte> kimFace = new Image<Bgr, byte>(kimFile).GetSubRect(kimRectangle);

            obamaFace.Save("G:\\facedata\\obama_rect.jpg");
            kimFace.Save("G:\\facedata\\kim_rect.jpg");

            FaceIntegration faceIntegration = new FaceIntegration(
                obamaFace,
                kimFace,
                obamaLandmark,
                kimLandmark,
                new Size(300, 300),
                0.5);
            Image<Bgr, byte> dstFace = faceIntegration.integrateFace();
            dstFace.Save("G:\\facedata\\obama_kim.jpg");

            //FaceRectangle[] faceRectangles;
            //FaceLandmarks[] faceLandmarks;
            //string filePath = "G:\\data\\Big-Bang-Theory-6.jpg";
            //runFaceAPI(filePath, out faceRectangles, out faceLandmarks);
            //convertLandmarkFormation(ref faceLandmarks[0], ref faceRectangles[0]);
            //Rectangle faceRect = convertRectangleFormation(faceRectangles[0]);
            //Image<Bgr, byte> srcImg1 = new Image<Bgr, byte>(filePath);
            //Image<Bgr, byte> faceImg1 = srcImg1.GetSubRect(faceRect);
        }

        private static void runFaceAPI(
            string _filePath, 
            out FaceRectangle[] _rects, 
            out FaceLandmarks[] _landmarks)
        {
            string key = "1cdb412565ae43879ea8133525e89040";
            FaceAPI faceAPI = new FaceAPI(key);
            var detectResult = faceAPI.detectFaces(_filePath);
            Task.WaitAll(detectResult);
            if (detectResult.Result)
            {
                _rects = faceAPI.getFaceRectangles();
                _landmarks = faceAPI.getFaceLandmarks();
            }
            else
            {
                _rects = new FaceRectangle[0];
                _landmarks = new FaceLandmarks[0];
            }
        }

        private static PointF[] convertLandmarkFormation(
            ref FaceLandmarks _landmarks,
            ref FaceRectangle _rectangle)
        {
            PointF[] retLandmarks = new PointF[3];
            retLandmarks[0] = convertPointFormation(_landmarks.EyeLeftInner, _rectangle);
            retLandmarks[1] = convertPointFormation(_landmarks.EyeRightInner, _rectangle);
            retLandmarks[2] = convertPointFormation(_landmarks.NoseTip, _rectangle);
            return retLandmarks;
        }

        private static PointF convertPointFormation(
            FeatureCoordinate _landmark,
            FaceRectangle _rectangle)
        {
            PointF retPoint = new PointF();
            retPoint.X = (float)((_landmark.X - _rectangle.Left) / _rectangle.Width);
            retPoint.Y = (float)((_landmark.Y - _rectangle.Top) / _rectangle.Height);
            return retPoint;
        }

        private static Rectangle convertRectangleFormation(
            FaceRectangle _rectangle)
        {
            return new Rectangle(
                _rectangle.Left,
                _rectangle.Top,
                _rectangle.Width,
                _rectangle.Height);
        }
    }
}