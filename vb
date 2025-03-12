import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import javax.xml.stream.XMLStreamException;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.sql.SQLException;
import java.util.*;

class StaXCallbackImplTest {

    @InjectMocks
    private StaXCallbackImpl staXCallbackImpl;

    @Mock
    private RSDao rsDao;

    @Mock
    private StaXBuilder staXBuilder;

    @BeforeEach
    void setup() {
        MockitoAnnotations.openMocks(this);
    }

    /**
     * Test: `hasDaoException == true` should add `PARTIAL_SUCCESS` and `DATASOURCE_ERROR`
     */
    @Test
    void testGenerateReportDetails_HasDaoException() throws Exception {
        when(rsDao.fetchRestrictionType(any())).thenReturn("Type1");

        Method method = StaXCallbackImpl.class.getDeclaredMethod("generateReportDetails", StaXBuilder.class, boolean.class, Set.class);
        method.setAccessible(true);

        method.invoke(staXCallbackImpl, staXBuilder, true, new HashSet<>());

        verify(staXBuilder).startElement(ExtractorConstants.REPORT_DETAILS);
        verify(staXBuilder).addElement(ExtractorConstants.TYPE_OF_RESTRICTION, "Type1");
        verify(staXBuilder).addElement(ExtractorConstants.EXTRACT_STATUS, ExtractorConstants.PARTIAL_SUCCESS);
        verify(staXBuilder).addElement(ExtractorConstants.FAILURE_REASON, ExtractorConstants.DATASOURCE_ERROR);
        verify(staXBuilder).endElement();
    }

    /**
     * Test: `hasDaoException == false` but `incorrectRICs` exist → should list incorrect RICs
     */
    @Test
    void testGenerateReportDetails_IncorrectRICs() throws Exception {
        when(rsDao.fetchRestrictionType(any())).thenReturn("Type1");

        Set<String> incorrectRICs = new HashSet<>();
        incorrectRICs.add("RIC123");
        incorrectRICs.add("RIC456");

        Method method = StaXCallbackImpl.class.getDeclaredMethod("generateReportDetails", StaXBuilder.class, boolean.class, Set.class);
        method.setAccessible(true);

        method.invoke(staXCallbackImpl, staXBuilder, false, incorrectRICs);

        verify(staXBuilder).startElement(ExtractorConstants.REPORT_DETAILS);
        verify(staXBuilder).addElement(ExtractorConstants.TYPE_OF_RESTRICTION, "Type1");
        verify(staXBuilder).addElement(ExtractorConstants.EXTRACT_STATUS, ExtractorConstants.SUCCESS);
        verify(staXBuilder).addElement(eq(ExtractorConstants.FAILURE_REASON), contains("RIC123,RIC456"));
        verify(staXBuilder).endElement();
    }
}
