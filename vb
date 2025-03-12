import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.Test;

class RSDaoExceptionTest {

    @Test
    void testConstructorWithMessage() {
        String message = "Test exception message";
        RSDaoException exception = new RSDaoException(message);
        assertEquals(message, exception.getMessage());
    }

    @Test
    void testConstructorWithMessageAndCause() {
        String message = "Test exception message";
        Throwable cause = new RuntimeException("Cause message");
        RSDaoException exception = new RSDaoException(message, cause);
        assertEquals(message, exception.getMessage());
        assertEquals(cause, exception.getCause());
    }

    @Test
    void testConstructorWithCause() {
        Throwable cause = new RuntimeException("Cause message");
        RSDaoException exception = new RSDaoException(cause);
        assertEquals(cause, exception.getCause());
    }
}
