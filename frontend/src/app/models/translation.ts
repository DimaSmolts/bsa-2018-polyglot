import { TranslationType } from "./TranslationType";

import { AdditionalTranslation } from "./additionalTranslation";

export interface Translation {

    id: string;
    userId: number;
    translationValue: string;
    languageId: number;
    createdOn: Date;

    history: AdditionalTranslation[];
    optionalTranslations: AdditionalTranslation[]

    Type: TranslationType

    assignedTranslatorId: number;
    assignedTranslatorName: string;
    assignedTranslatorAvatarUrl: string;
}

