using UnityEngine;
using System.Collections;

public class Pong : MonoBehaviour {

	Vector2 screenSize;
	public static float gameScale = 1.0f;

	public Texture texture;
	Rect texCoordBall = new Rect(0,0,0.5f,1);
	Rect texCoordBlock = new Rect(0.5f,0,0.5f,1);
	/*const */float cellSize = 16;

	bool pause = false;

	struct Ball {
		public static float size;
		public static float halfSize;
		public Vector2 pos;
		public Vector2 acc;
		public float speed;
		public bool Move (ref Rect colliArea) {
			bool hit = false;
			pos += acc * gameScale;
			if (pos.x < colliArea.x) {
				acc.x = -acc.x;
				pos.x = colliArea.x+halfSize;
				hit = true;
			} else if (pos.x > colliArea.x+colliArea.width) {
				acc.x = -acc.x;
				pos.x = colliArea.x+colliArea.width-halfSize;
				hit = true;
			}
			if (pos.y < colliArea.y) {
				acc.y = -acc.y;
				pos.y = colliArea.y+halfSize;
				hit = true;
			} else if (pos.y > colliArea.y+colliArea.height) {
				acc.y = -acc.y;
				pos.y = colliArea.y+colliArea.height-halfSize;
				hit = true;
			}
			return hit;
		}
	};
	const int ballCount = 5;
	Ball[] balls = new Ball[ballCount];

	struct Paddle {
		public Vector2 size;
		public Vector2 pos;
	};
	Paddle paddle;
	
	struct Stage {
		public Rect rect;
		public Rect ballArea;
		public Rect paddleArea;
	};
	Stage stage;
			

	// Use this for initialization
	void Start () {
		// Init Layout
	 	ChangedScreenSize();

	 	// Init Game
	 	for (int i=0; i<ballCount; ++i) {
		 	balls[i].pos = new Vector2(paddle.pos.x, paddle.pos.y-Ball.halfSize);
		 	balls[i].acc = new Vector2(1.0f+Random.value*4.0f, -3.0f+Random.value*2.0f);
		 	balls[i].speed = 6.0f;
		 	balls[i].acc.Normalize();
		 	balls[i].acc *= balls[i].speed;
		 }
	 }
	 
	 void ChangedScreenSize () {
 	 	screenSize.x = Screen.width;
	 	screenSize.y = Screen.height;

	 	// Init each size
	 	gameScale = (Screen.width < Screen.height) ? Screen.width/320.0f : Screen.height/320.0f;
	 	cellSize = 16.0f*gameScale;
		Ball.size = cellSize;
		Ball.halfSize = Ball.size*0.5f;
		paddle.size = new Vector2(cellSize*6.0f, cellSize);

		// Layout
	 	stage.rect = new Rect(0,56*gameScale, screenSize.x, screenSize.y-56*gameScale);
	 	stage.ballArea = new Rect(stage.rect.x+cellSize+Ball.halfSize,stage.rect.y+cellSize+Ball.halfSize, stage.rect.width-cellSize*2-Ball.size,stage.rect.height-cellSize-Ball.size);
	 	stage.paddleArea = new Rect(stage.rect.x+cellSize+paddle.size.x*0.5f,stage.rect.y*0.5f, stage.rect.width-cellSize*2-paddle.size.x,stage.rect.height*0.5f);
	 	paddle.pos = new Vector2(screenSize.x*0.5f, screenSize.y-paddle.size.y*4);
	 }
	
	public bool Pause (bool pause) {
		this.pause = pause;
		return this.pause;
	}

	public bool TouchEvent(ref Vector2 touchPos, bool began) {
		if (pause == false) {
			if (touchPos.y > Screen.height*0.5f) {
				paddle.pos.x = touchPos.x;
				if (paddle.pos.x < stage.paddleArea.x) {
					paddle.pos.x = stage.paddleArea.x;
				} else if (paddle.pos.x > stage.paddleArea.x+stage.paddleArea.width) {
					paddle.pos.x = stage.paddleArea.x+stage.paddleArea.width;
				}
				return true;
			}
		}
		return false;
	}

	void FixedUpdate () {
		if (pause) {
			return;
		}
	 	for (int i=0; i<ballCount; ++i) {
			balls[i].Move(ref stage.ballArea);
			// Collision Ball to Paddle
			if ((balls[i].acc.y > 0.0f) && (balls[i].pos.y+Ball.halfSize > paddle.pos.y) && (balls[i].pos.y-Ball.halfSize < paddle.pos.y+paddle.size.y)) {
				if ((balls[i].pos.x+Ball.halfSize >= paddle.pos.x-paddle.size.x*0.5f) && (balls[i].pos.x-Ball.halfSize <= paddle.pos.x+paddle.size.x*0.5f)) {
					balls[i].acc.y = -balls[i].acc.y;
//					balls[i].pos.y = paddle.pos.y-Ball.halfSize;
					//
					float r = (paddle.pos.x-balls[i].pos.x)/(paddle.size.x*0.5f);
					balls[i].acc.x = balls[i].acc.y * r;
					balls[i].acc.Normalize();
					balls[i].acc *= balls[i].speed;
				}
			}
		}
	}
	void Update () {
		if ((Screen.width != screenSize.x) || (Screen.height != screenSize.y)) {
			ChangedScreenSize();
		}
		if (pause) {
			return;
		}
	}

	void OnGUI () {
		// Stage
		GUI.DrawTextureWithTexCoords(new Rect(stage.rect.x, stage.rect.y, stage.rect.width, cellSize), texture, texCoordBlock);
		GUI.DrawTextureWithTexCoords(new Rect(stage.rect.x, stage.rect.y+cellSize, cellSize, stage.rect.height-cellSize), texture, texCoordBlock);
		GUI.DrawTextureWithTexCoords(new Rect(stage.rect.x+stage.rect.width-cellSize, stage.rect.y+cellSize, cellSize, stage.rect.height-cellSize), texture, texCoordBlock);
		//GUI.DrawTextureWithTexCoords(new Rect(stage.rect.x, stage.rect.y+stage.rect.height-cellSize, stage.rect.width, cellSize), texture, texCoordBlock);
		
		// Paddle
		GUI.DrawTextureWithTexCoords(new Rect(paddle.pos.x-paddle.size.x*0.5f, paddle.pos.y, paddle.size.x, paddle.size.y), texture, texCoordBlock);

		// Balls
		foreach (Ball ball in balls) {
			GUI.DrawTextureWithTexCoords(new Rect(ball.pos.x-Ball.halfSize, ball.pos.y-Ball.halfSize, Ball.size, Ball.size), texture, texCoordBall);
		}
	}
}
