import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import java.lang.reflect.Method;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

import javax.xml.stream.XMLStreamException;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockedStatic;
import org.mockito.MockitoAnnotations;

// Replace with your actual package names
// import com.db.fusion.rs.service.impl.StaXCallbackImpl;
// import com.db.fusion.rs.stax.StaXBuilder;
// import com.db.fusion.rs.model.RestrictedSecurity;
// import com.db.fusion.rs.util.ExtractorConstants;
// import com.db.fusion.rs.util.RSReportUtil;

class StaXCallbackImplRestrictedSecuritiesListTest {

    @InjectMocks
    private StaXCallbackImpl staXCallbackImpl;

    @Mock
    private StaXBuilder staXBuilder;

    // Create a dummy RestrictedSecurity using a mock.
    @Mock
    private RestrictedSecurity restrictedSecurity;

    @BeforeEach
    void setup() {
        MockitoAnnotations.openMocks(this);
        // Allow method chaining on StaXBuilder:
        when(staXBuilder.startElement(anyString())).thenReturn(staXBuilder);
        when(staXBuilder.addElement(anyString(), anyString())).thenReturn(staXBuilder);
    }

    /**
     * Test for generaterestrictedSecuritiesList:
     * Verifies that the proper start/end elements are created and that the static utility RSReportUtil is called
     * for ISINs, CUSIPs, RICs, and WPKs.
     */
    @Test
    void testGenerateRestrictedSecuritiesList() throws Exception {
        // Stub the RestrictedSecurity to return dummy values
        when(restrictedSecurity.getSecurityDescription()).thenReturn("TestSecurity");
        when(restrictedSecurity.getISINS()).thenReturn(Arrays.asList("ISIN1", "ISIN2"));
        when(restrictedSecurity.getCUSIPS()).thenReturn(Collections.singletonList("CUSIP1"));
        when(restrictedSecurity.getCorrectRICs()).thenReturn(Arrays.asList("RIC1", "RIC2"));
        // Assume one of the WPK loops is non-empty (the one that calls RSReportUtil)
        when(restrictedSecurity.getWPKS()).thenReturn(Collections.emptyList());
        when(restrictedSecurity.getWPKs()).thenReturn(Collections.singletonList("WPK1"));

        List<RestrictedSecurity> restrictions = Arrays.asList(restrictedSecurity);

        // Use Mockito's static mocking to capture calls to RSReportUtil
        try (MockedStatic<RSReportUtil> mockedRSReportUtil = mockStatic(RSReportUtil.class)) {
            // Obtain the protected method via reflection
            Method method = StaXCallbackImpl.class.getDeclaredMethod("generaterestrictedSecuritiesList", StaXBuilder.class, List.class);
            method.setAccessible(true);

            // Invoke the method under test
            method.invoke(staXCallbackImpl, staXBuilder, restrictions);

            // Verify that the StaXBuilder methods are called properly:
            verify(staXBuilder).startElement(ExtractorConstants.RESTRICTED_SECURITY);
            verify(staXBuilder).addElement(ExtractorConstants.SECURITY_DESCRIPTION, "TestSecurity");
            verify(staXBuilder).endElement();

            // Verify RSReportUtil calls for each security identifier type:
            // For ISINS:
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.ISIN, "ISIN1"));
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.ISIN, "ISIN2"));
            // For CUSIPs:
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.CUSIP, "CUSIP1"));
            // For RICs:
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.RIC, "RIC1"));
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.RIC, "RIC2"));
            // For WPKs (from getWPKs())
            mockedRSReportUtil.verify(() -> RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.WPK, "WPK1"));
        }
    }
}
