using UnityEngine;
using TMPro;

public class Paper : MonoBehaviour
{
    public GameObject letterUI; // UI để hiển thị lá thư
    public TextMeshProUGUI letterText; // Text hiển thị nội dung lá thư
    public TextMeshProUGUI interactionText; // Text hiển thị "Nhấn E để đọc"

    private bool isNearLetter = false;
    private bool isReadingLetter = false; // Biến kiểm soát việc đang đọc lá thư

    void Start()
    {
        letterUI.SetActive(false); // Ẩn UI lá thư ban đầu
        interactionText.gameObject.SetActive(false); // Ẩn thông báo ban đầu
    }

    void Update()
    {
        if (isNearLetter && Input.GetKeyDown(KeyCode.E)) // Nếu có nhân vật ở gần và nhấn E
        {
            letterUI.SetActive(!letterUI.activeSelf); // Bật/tắt UI lá thư

            if (letterUI.activeSelf)
            {
                interactionText.gameObject.SetActive(false); // Ẩn thông báo khi UI mở
                isReadingLetter = true; // Đánh dấu đang đọc lá thư
            }
            else
            {
                interactionText.gameObject.SetActive(true); // Hiện lại thông báo khi UI đóng
                isReadingLetter = false; // Đánh dấu không còn đọc lá thư
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu là người chơi
        {
            if (!letterUI.activeSelf) // Chỉ hiện thông báo khi UI lá thư chưa mở
            {
                interactionText.gameObject.SetActive(true); // Hiện thông báo "Nhấn E để đọc"
            }
            isNearLetter = true; // Đánh dấu đã ở gần lá thư
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionText.gameObject.SetActive(false); // Ẩn thông báo khi nhân vật đi xa
            isNearLetter = false;

            if (letterUI.activeSelf)
            {
                letterUI.SetActive(false); // Đóng UI lá thư khi rời xa
                isReadingLetter = false; // Đánh dấu không còn đọc lá thư
            }
        }
    }
}
