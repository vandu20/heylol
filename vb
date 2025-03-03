import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;
import static org.mockito.ArgumentMatchers.*;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.sql.Connection;
import java.sql.SQLException;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.mockito.MockedStatic;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@ExtendWith(MockitoExtension.class)
public class RSReportGeneratorCallbackTest {

    private static final Logger LOGGER = LoggerFactory.getLogger(RSReportGeneratorCallbackTest.class);

    @Mock
    private Connection connection;

    @Mock
    private RSExtractorService rsExtractor;

    @Mock
    private RSDaoImpl rsDao;

    @InjectMocks
    private GenerateRSReportServiceImpl generateRSReportService;

    private static final String TEST_FILE_PATH = "src/main/resources/restricted_securities.xml";

    @BeforeEach
    void setup() {
        // Ensure test files are deleted before each test
        try {
            Path path = Paths.get(TEST_FILE_PATH);
            Files.deleteIfExists(path);
        } catch (IOException e) {
            LOGGER.error("Error cleaning up test files: " + e.getMessage(), e);
        }
    }

    @Test
    void testGenerateRSReport_Success() throws SQLException, IOException {
        // Arrange
        String mockRestrictionType = "Restricted";
        when(rsDao.fetchRestrictionType(connection)).thenReturn(mockRestrictionType);

        ByteArrayOutputStream mockOutputStream = new ByteArrayOutputStream();
        mockOutputStream.write("Test XML Data".getBytes());

        when(rsExtractor.extract(eq(mockRestrictionType), any())).thenReturn(mockOutputStream);

        // Act
        String resultFilePath = generateRSReportService.generateRSReport();

        // Assert
        assertNotNull(resultFilePath);
        assertEquals(TEST_FILE_PATH, resultFilePath);
        assertTrue(Files.exists(Paths.get(TEST_FILE_PATH)));

        // Verify interactions
        verify(rsDao).fetchRestrictionType(connection);
        verify(rsExtractor).extract(eq(mockRestrictionType), any());
    }

    @Test
    void testGenerateRSReport_ThrowsSQLException() throws SQLException {
        // Arrange
        when(rsDao.fetchRestrictionType(connection)).thenThrow(new SQLException("DB error"));

        // Act & Assert
        RuntimeException thrown = assertThrows(RuntimeException.class, () -> generateRSReportService.generateRSReport());
        assertEquals("java.sql.SQLException: DB error", thrown.getCause().toString());

        // Verify that extraction was never called
        verify(rsExtractor, never()).extract(anyString(), any());
    }

    @Test
    void testGenerateRSReport_ThrowsIOException() throws SQLException, IOException {
        // Arrange
        String mockRestrictionType = "Restricted";
        when(rsDao.fetchRestrictionType(connection)).thenReturn(mockRestrictionType);

        ByteArrayOutputStream mockOutputStream = new ByteArrayOutputStream();
        mockOutputStream.write("Test XML Data".getBytes());

        when(rsExtractor.extract(eq(mockRestrictionType), any())).thenReturn(mockOutputStream);

        // Use static mocking for Files.copy()
        try (MockedStatic<Files> mockedFiles = mockStatic(Files.class)) {
            mockedFiles.when(() -> Files.copy(any(InputStream.class), any(Path.class), any(StandardCopyOption.class)))
                       .thenThrow(new IOException("File write error"));

            // Act & Assert
            IOException thrown = assertThrows(IOException.class, () -> generateRSReportService.generateRSReport());
            assertEquals("File write error", thrown.getMessage());

            // Verify interactions
            verify(rsDao).fetchRestrictionType(connection);
            verify(rsExtractor).extract(eq(mockRestrictionType), any());
        }
    }
}
