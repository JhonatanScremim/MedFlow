type FeedbackVariant = "info" | "success" | "error";

interface FeedbackProps {
  message: string;
  variant?: FeedbackVariant;
}

export function Feedback({ message, variant = "info" }: FeedbackProps) {
  return (
    <p
      className={`feedback ${variant !== "info" ? variant : ""}`.trim()}
      role="alert"
      aria-live="polite"
    >
      {message}
    </p>
  );
}
