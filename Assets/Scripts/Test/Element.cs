using System.Collections;
using System.Collections.Generic;
using OPH.Collision.QuadTree;
using UnityEngine;

namespace OPH.Example {
    public class Element : MonoBehaviour, IRect {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        protected Vector3 Dir;
        public Camera cam;
        public float Speed = 1;
        public Vector2 size;
        public Renderer render;
        protected Material normal;
        protected Material click;

        private void Awake() {
            Width = transform.localScale.x;
            Height = transform.localScale.y;
            size.x = transform.localScale.x;
            size.y = transform.localScale.y;
            if (render) {
                normal = Instantiate(render.material);
                click = Instantiate(render.material);
                click.color = Color.red;
            }
        }

        private void Update() {
            UpdatePoint();
            Vector3 v = Speed * Time.deltaTime * Dir;
            Vector3 final = transform.position + v;
            if (final.x - Width / 2 < -size.x / 2) Dir.x = -Dir.x;
            if (final.x + Width / 2 > size.x / 2) Dir.x = -Dir.x;
            if (final.y - Width / 2 < -size.y / 2) Dir.y = -Dir.y;
            if (final.y + Height / 2 > size.y / 2) Dir.y = -Dir.y;
            transform.position += Speed * Time.deltaTime * Dir;
        }

        private void OnMouseDrag() {
            if (cam) {
                Vector3 sPos = Input.mousePosition;
                sPos.z = -cam.transform.position.z;
                transform.position = cam.ScreenToWorldPoint(sPos);
            }
        }

        public void Init(Vector2 size) {
            UpdatePoint();
            InitDir();
            Width = Height = transform.localScale.x;
            this.size = size;
        }

        private void UpdatePoint() {
            X = transform.position.x;
            Y = transform.position.y;
        }

        public void InitDir() {
            Dir = Random.insideUnitCircle.normalized;
        }

        public void Click(bool off) {
            render.material = off ? click : normal;
        }
    }
}
