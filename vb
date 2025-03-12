import com.db.fusion.rs.model.RestrictedSecurity;
import com.db.fusion.rs.service.impl.StaXCallbackImpl;
import com.db.fusion.rs.stax.StaXBuilder;
import com.db.fusion.rs.util.ExtractorConstants;
import com.db.fusion.rs.util.RSReportUtil;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.jupiter.MockitoExtension;

import javax.xml.stream.XMLStreamException;
import java.util.Arrays;
import java.util.ArrayList;
import java.util.List;

import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
public class StaXCallbackImplTest {

    @InjectMocks
    private StaXCallbackImpl staXCallbackImpl;

    @Mock
    private StaXBuilder staXBuilder;

    @Mock
    private RestrictedSecurity restrictedSecurity;

    @BeforeEach
    void setup() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void testGenerateRestrictedSecuritiesList_Success() throws XMLStreamException {
        // Mock RestrictedSecurity object
        List<RestrictedSecurity> restrictions = new ArrayList<>();
        when(restrictedSecurity.getSecurityDescription()).thenReturn("Test Security");
        when(restrictedSecurity.getISINS()).thenReturn(Arrays.asList("ISIN1", "ISIN2"));
        when(restrictedSecurity.getCUSIPS()).thenReturn(Arrays.asList("CUSIP1"));
        when(restrictedSecurity.getCorrectRICs()).thenReturn(Arrays.asList("RIC1"));
        when(restrictedSecurity.getWPKS()).thenReturn(Arrays.asList("WPK1"));

        restrictions.add(restrictedSecurity);

        // Mock StaXBuilder behavior
        when(staXBuilder.startElement(ExtractorConstants.RESTRICTED_SECURITY)).thenReturn(staXBuilder);
        when(staXBuilder.addElement(ExtractorConstants.SECURITY_DESCRIPTION, "Test Security")).thenReturn(staXBuilder);
        
        // Call the method under test
        staXCallbackImpl.generateRestrictedSecuritiesList(staXBuilder, restrictions);

        // Verify interactions
        verify(staXBuilder, times(1)).startElement(ExtractorConstants.RESTRICTED_SECURITY);
        verify(staXBuilder, times(1)).addElement(ExtractorConstants.SECURITY_DESCRIPTION, "Test Security");

        verify(restrictedSecurity, times(1)).getISINS();
        verify(restrictedSecurity, times(1)).getCUSIPS();
        verify(restrictedSecurity, times(1)).getCorrectRICs();
        verify(restrictedSecurity, times(1)).getWPKS();

        verify(staXBuilder, times(1)).endElement();
    }
}
