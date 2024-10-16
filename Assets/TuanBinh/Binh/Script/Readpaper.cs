using UnityEngine;
using TMPro;
using System.Collections;

public class LetterInteraction : MonoBehaviour
{
    public GameObject letterUI; // UI để hiển thị lá thư
    public TextMeshProUGUI letterText; // Text hiển thị nội dung lá thư ban đầu
    public TextMeshProUGUI interactionText; // Text hiển thị "Nhấn E để đọc"
    public TextMeshProUGUI newLetterContent; // Nội dung mới sẽ hiển thị
    public GameObject firstPanel; // Panel đầu tiên sẽ hiện sau 3 giây
    public GameObject secondPanel; // Panel thứ hai sẽ hiện sau 2 giây từ khi panel đầu tiên xuất hiện
    private bool isNearLetter = false;
    private bool isReadingLetter = false;
    private bool hasReadLetter = false; // Biến kiểm soát việc đã đọc lá thư
    private GameObject currentCharacter = null; // Lưu trữ nhân vật hiện tại đang ở gần

    void Start()
    {
        letterUI.SetActive(false); // Ẩn UI lá thư ban đầu
        interactionText.gameObject.SetActive(false); // Ẩn thông báo ban đầu
        newLetterContent.gameObject.SetActive(false); // Ẩn nội dung mới khi chưa đọc
        firstPanel.SetActive(false); // Ẩn panel đầu tiên khi chưa đọc
        secondPanel.SetActive(false); // Ẩn panel thứ hai khi chưa đọc
    }

    void Update()
    {
        if (isNearLetter && Input.GetKeyDown(KeyCode.E) && !hasReadLetter) // Nếu có nhân vật ở gần, nhấn E và chưa đọc lá thư
        {
            letterUI.SetActive(!letterUI.activeSelf); // Bật/tắt UI lá thư

            // Ẩn hoặc hiện thông báo tùy theo trạng thái của UI lá thư
            if (letterUI.activeSelf)
            {
                interactionText.gameObject.SetActive(false); // Ẩn thông báo khi UI mở
                isReadingLetter = true; // Đánh dấu đang đọc lá thư
                StartCoroutine(ChangeLetterContentAndShowFirstPanel(3)); // Gọi Coroutine để đổi nội dung và hiển thị panel đầu tiên sau 3 giây
            }
            else
            {
                interactionText.gameObject.SetActive(true); // Hiện lại thông báo khi UI đóng
                isReadingLetter = false; // Đánh dấu không đọc lá thư nữa
                StopAllCoroutines(); // Ngừng tất cả Coroutine khi UI đóng
                letterText.gameObject.SetActive(true); // Hiện lại nội dung lá thư ban đầu khi đóng UI
                newLetterContent.gameObject.SetActive(false); // Ẩn nội dung mới khi đóng UI
                firstPanel.SetActive(false); // Ẩn panel đầu tiên khi đóng UI
                secondPanel.SetActive(false); // Ẩn panel thứ hai khi đóng UI
            }
        }
    }

    IEnumerator ChangeLetterContentAndShowFirstPanel(float delay)
    {
        yield return new WaitForSeconds(delay); // Đợi 3 giây
        if (isReadingLetter) // Kiểm tra lại nếu vẫn đang đọc
        {
            yield return StartCoroutine(FadeOut(letterText)); // Fade out nội dung ban đầu
            letterText.gameObject.SetActive(false); // Ẩn nội dung ban đầu
            newLetterContent.gameObject.SetActive(true); // Hiện nội dung mới sau khi đổi
            firstPanel.SetActive(true); // Hiện panel đầu tiên sau khi đổi nội dung
            StartCoroutine(FadeOut(firstPanel)); // Fade out panel đầu tiên sau khi hiện
            StartCoroutine(ShowSecondPanelAfterDelay(2)); // Gọi Coroutine để hiện panel thứ hai sau 2 giây
        }
    }

    IEnumerator ShowSecondPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Đợi thêm 2 giây
        if (isReadingLetter) // Kiểm tra nếu vẫn đang đọc
        {
            secondPanel.SetActive(true); // Hiển thị panel thứ hai
            // Không gọi fade out ở đây để giữ panel hai hiển thị
        }
    }

    IEnumerator FadeOut(TextMeshProUGUI text, float fadeDuration = 1f)
    {
        Color originalColor = text.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime); // Giảm alpha từ 1 đến 0
            yield return null; // Chờ một frame
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // Đảm bảo alpha là 0 ở cuối
    }

    IEnumerator FadeOut(GameObject panel, float fadeDuration = 1f)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>(); // Thêm CanvasGroup nếu chưa có
        }

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            canvasGroup.alpha = 1 - normalizedTime; // Giảm alpha từ 1 đến 0
            yield return null; // Chờ một frame
        }
        canvasGroup.alpha = 0; // Đảm bảo alpha là 0 ở cuối
        // panel.SetActive(false); // Để dòng này bình luận lại để panel không bị ẩn
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu nhân vật có Tag là "Player"
        if (other.CompareTag("Player") && !hasReadLetter) // Chỉ hiện thông báo nếu chưa đọc lá thư
        {
            if (!letterUI.activeSelf) // Chỉ hiện thông báo khi UI lá thư chưa mở
            {
                interactionText.gameObject.SetActive(true); // Hiện thông báo "Nhấn E để đọc"
            }
            isNearLetter = true;
            currentCharacter = other.gameObject; // Lưu nhân vật hiện tại đang ở gần
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentCharacter)
        {
            interactionText.gameObject.SetActive(false); // Ẩn thông báo khi nhân vật đi xa
            isNearLetter = false;
            currentCharacter = null; // Xóa thông tin về nhân vật hiện tại

            if (letterUI.activeSelf)
            {
                letterUI.SetActive(false); // Đóng UI lá thư khi rời xa
                isReadingLetter = false; // Đánh dấu không còn đọc lá thư
                StopAllCoroutines(); // Ngừng Coroutine khi UI đóng
                letterText.gameObject.SetActive(true); // Hiện lại nội dung ban đầu khi rời xa
                newLetterContent.gameObject.SetActive(false); // Ẩn nội dung mới khi rời xa
                firstPanel.SetActive(false); // Ẩn panel đầu tiên khi rời xa
                secondPanel.SetActive(false); // Ẩn panel thứ hai khi rời xa
            }
        }
    }
}
