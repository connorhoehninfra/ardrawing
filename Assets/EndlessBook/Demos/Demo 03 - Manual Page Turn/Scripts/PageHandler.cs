namespace echo17.EndlessBook.PageHandler
{
	using UnityEngine;
	using echo17.EndlessBook;


	public class PageHandler : MonoBehaviour
	{


		/// <summary>
		/// The book to control
		/// </summary>
		public EndlessBook book;

		/// <summary>
		/// The speed to play the page turn animation when the mouse is let go
		/// </summary>
		public float turnStopSpeed;

		/// <summary>
		/// If this is turned on, then the page will reverse direction
		/// if the page is not past the midway point of the book.
		/// </summary>
		public bool reversePageIfNotMidway = true;

		/// <summary>
		/// The box collider to check for mouse motions
		/// </summary>
		protected BoxCollider boxCollider;

		/// <summary>
		/// Whether the mouse is currently down
		/// </summary>
		public bool isHoldingPage;
		public float controllerPos;
		public LayerMask BooklayerMask;
		GameObject handInUse;

		public float xLeft, xRight, xCurrent;


		private void Start()
		{
			boxCollider = gameObject.GetComponent<BoxCollider>();

		}


		private void InvokeOnMouseDown()
		{
			// get the scaled position of the controller
			var controllerPos = GetScaledControllerPosition();

			// calculate the direction of the page turn based on the controller position
			var direction = controllerPos > 0.5f ? Page.TurnDirectionEnum.TurnForward : Page.TurnDirectionEnum.TurnBackward;

			// tell the book to start turning a page manually
			book.TurnPageDragStart(direction);

			// We are holding the page
			isHoldingPage = true;
		}


		void InvokeOnMouseDrag()
		{
			// get the scaled controller position 
			controllerPos = GetScaledControllerPosition();

			// tell the book to move the manual page drag to the normalized time
			book.TurnPageDrag(controllerPos);
		}

		void InvokeOnMouseUp()
		{
			// tell the book to stop manual turning.
			// if we have reversePageIfNotMidway on, then we look to see if we have turned past the midway point.
			// if not, we reverse the page.
			book.TurnPageDragStop(turnStopSpeed, PageTurnCompleted, reverse: reversePageIfNotMidway ? (book.TurnPageDragNormalizedTime < 0.5f) : false);

			// We are not holding the page anymore
			isHoldingPage = false;

			//Set the selected hand to null
			handInUse = null;
		}


		/// <summary>
		/// Gets the position of the controller relative to the ends of the book
		/// </summary>
		/// <returns></returns>
		protected virtual float GetScaledControllerPosition()
		{

			RaycastHit hit;
			float xStartPoint = xLeft = transform.position.x - (transform.localScale.x / 2f);
			float xEndPoint = xRight = transform.position.x + (transform.localScale.x / 2f);


			// cast a ray and see where it hits
			if (Physics.Raycast(handInUse.transform.position, Vector3.down, out hit, 100f, BooklayerMask))
				return Mathf.Clamp((hit.point.x - xStartPoint) / (xEndPoint - xStartPoint), 0f, 1f);

			var pos = handInUse.transform.position;
			pos.y = transform.position.y;

			//Check if the controller is still in the x-axis range of the book 
			if (pos.x > xStartPoint && pos.x < xEndPoint)
				return Mathf.Clamp((pos.x - xStartPoint) / (xEndPoint - xStartPoint), 0f, 1f);
			else
				// if we didn't hit the collider, then return bounds
				return pos.x < transform.position.x ? 0 : 1;
		}

		/// <summary>
		/// Called when the page completes its manual turn
		/// </summary>
		protected virtual void PageTurnCompleted(int leftPageNumber, int rightPageNumber)
		{
			//isTurning = false;
		}



		private void Update()
		{
			if (!isHoldingPage) return;

			if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
				InvokeOnMouseUp();
			else
				InvokeOnMouseDrag();
		}


		private void OnTriggerStay(Collider other)
		{
			if (other.tag != "ControllerGrab") return;



			if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
			{
				handInUse = other.gameObject;
				InvokeOnMouseDown();
			}
		}

	}

}
